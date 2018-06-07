using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahloService;
using MahloService.Settings;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Utilities;
using Newtonsoft.Json;
using PropertyChanged;

namespace MahloService.Logic
{
  [AddINotifyPropertyChangedInterface]
  abstract class MeterLogic<Model> : IMeterLogic<Model>, IDisposable
    where Model : MahloRoll, new()
  {
    private ISewinQueue sewinQueue;
    private IMeterSrc<Model> srcData;
    private IServiceSettings appInfo;
    private IProgramState programState;

    private bool checkRollSize = true;
    private int rollCount;
    private int styleCount;
    //private double meterResetAtLength;
    private int feetCounterAtRollStart;

    private List<IDisposable> disposables = new List<IDisposable>();

    public MeterLogic(
      IMeterSrc<Model> srcData,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<Model> userAttentions,
      ICriticalStops<Model> criticalStops,
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
    {
      this.sewinQueue = sewinQueue;
      this.srcData = srcData;
      this.appInfo = appInfo;
      this.UserAttentions = userAttentions;
      this.CriticalStops = criticalStops;
      this.programState = programState;
      this.RestoreState();
      this.disposables.AddRange(new[]
        {
          Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
              h => this.srcData.PropertyChanged += h,
              h => this.srcData.PropertyChanged -= h)
            .Subscribe(args => this.OpcValueChanged(args.EventArgs.PropertyName)),

          Observable
            .Interval(TimeSpan.FromSeconds(1), schedulerProvider.WinFormsThread)
            .Subscribe(_ =>
            {
              this.Recipe = this.srcData.Recipe;
              this.IsManualMode = this.srcData.IsAutoMode;
            }),

          Observable
            .FromEvent(
              h => this.sewinQueue.QueueChanged += h,
              h => this.sewinQueue.QueueChanged -= h)
            .Subscribe(_ => this.SewinQueueChanged()),

          Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
              h => ((INotifyPropertyChanged)this.UserAttentions).PropertyChanged += h,
              h => ((INotifyPropertyChanged)this.UserAttentions).PropertyChanged -= h)
            .Subscribe(_ =>
            {
              this.IsMapValid &= this.UserAttentions.Any;
              this.IsChanged = true;
            }),

          Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
              h => this.CriticalStops.PropertyChanged += h,
              h => this.CriticalStops.PropertyChanged -= h)
            .Subscribe(_ =>
            {
              this.IsMapValid &= this.CriticalStops.Any;
              this.IsChanged = true;
            }),

          Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
              h => this.sewinQueue.PropertyChanged += h,
              h => this.sewinQueue.PropertyChanged -= h)
            .Subscribe(args => this.QueueMessage = this.sewinQueue.Message),
        });

    }

    public event Action<CarpetRoll> RollStarted;
    public event Action<CarpetRoll> RollFinished;

    /// <summary>
    /// Gets or sets a value indicating whether this object has been changed
    /// It is set by PropertyChangedFody and reset by Ipc.MahloServer
    /// </summary>
    public bool IsChanged { get; set; } = true;

    /// <summary>
    /// Get the roll that is currently being processed
    /// </summary>
    public CarpetRoll CurrentRoll { get; set; } = new CarpetRoll();

    [DependsOn(nameof(CurrentRoll))]
    public int CurrentRollIndex => this.sewinQueue.Rolls.IndexOf(this.CurrentRoll);

    public IUserAttentions UserAttentions { get; set; }

    public ICriticalStops CriticalStops { get; set; }

    public bool IsSeamDetected { get; set; }

    // Appear last
    [JsonProperty(Order = 1)]
    public abstract int MeasuredLength { get; set; }
    [JsonProperty(Order = 1)]
    public abstract int Speed { get; set; }
    [JsonProperty(Order = 1)]
    public abstract bool IsMapValid { get; set; }

    public bool IsManualMode { get; set; }

    public string Recipe { get; set; }

    public int PreviousRollLength { get; set; }

    public double MeasuredWidth { get; set; }

    public int RollChangesUntilCheckRequired
    {
      get => Math.Max(0, this.appInfo.CheckAfterHowManyRolls - this.rollCount);
      set => throw new NotImplementedException();
    }

    public int StyleChangesUntilCheckRequired
    {
      get => Math.Max(0, this.appInfo.CheckAfterHowManyStyles - this.styleCount);
      set => throw new NotImplementedException();
    }

    public string QueueMessage { get; set; }
    public bool IsMappingNow { get; set; }

    /// <summary>
    /// Called to start data collection
    /// </summary>
    public void Start()
    {
    }

    public void Dispose()
    {
      this.Dispose(true);
    }

    public bool GetIsMappingValid()
    {
      return !this.CurrentRoll.IsCheckRoll && !this.UserAttentions.Any && !this.CriticalStops.Any;
    }

    protected virtual void RestoreState()
    {
      var state = this.programState.GetSubState(nameof(MeterLogic<Model>), nameof(Model));
      this.checkRollSize = state.Get<bool?>(nameof(checkRollSize)) ?? true;
      this.rollCount = state.Get<int?>(nameof(rollCount)) ?? 0;
      this.styleCount = state.Get<int?>(nameof(styleCount)) ?? this.styleCount;
      this.feetCounterAtRollStart = state.Get<int?>(nameof(feetCounterAtRollStart)) ?? 0;

      string rollNo = state.Get<string>(nameof(this.CurrentRoll.RollNo)) ?? string.Empty;
      this.CurrentRoll = this.sewinQueue.Rolls.FirstOrDefault(roll => roll.RollNo == rollNo) ?? new CarpetRoll();

      // On startup, roll sequence should be verified
      this.UserAttentions.VerifyRollSequence = true;
    }

    protected virtual void Dispose(bool disposing)
    {
      // Save program state
      this.SaveState();
      this.disposables.ForEach(item => item.Dispose());
    }

    protected virtual void OnRollStarted(CarpetRoll carpetRoll)
    {
      this.RollStarted?.Invoke(carpetRoll);
    }

    protected virtual void OnRollFinished(CarpetRoll carpetRoll)
    {
      this.RollFinished?.Invoke(carpetRoll);
    }

    private void SaveState()
    {
      var state = programState.GetSubState(nameof(MeterLogic<Model>));
      state.Set(typeof(Model).Name, new
      {
        this.rollCount,
        this.styleCount,
        this.feetCounterAtRollStart,
        this.checkRollSize,
        this.CurrentRoll.RollNo,
      });
    }

    private void Simulate_lblMeasuredLen_TextChanged()
    {
      double keepOnLength = this.appInfo.SeamIndicatorKeepOnLength;

      if (this.MeasuredLength == 0)
      {
        return;
      }

      if (this.CurrentRoll.RollLength <= keepOnLength)
      {
        keepOnLength = this.CurrentRoll.RollLength / 2;
      }

      if (keepOnLength == 0)
      {
        keepOnLength = 5;
      }

      if (this.MeasuredLength >= keepOnLength)
      {
        //if (this.meterResetAtLength == 0)
        {
          this.IsSeamDetected = false;
        }

        this.srcData.SetMiscellaneousIndicator(false);
      }
    }

    private void SaveRealtimeDataToDataSet()
    {

    }

    private void SaveLineSpeedToFile()
    {

    }

    private void SaveRollMap()
    {

    }

    public virtual Task ApplyRecipe(string recipeName, bool isManualMode)
    {
      return Task.CompletedTask;
    }

    protected virtual void OpcValueChanged(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(this.srcData.FeetCounter):
          this.FeetCounterChanged((int)this.srcData.FeetCounter);
          break;

        case nameof(this.srcData.IsSeamDetected):
          this.SeamDetected(this.srcData.IsSeamDetected);
          break;

        case nameof(this.srcData.MeasuredWidth):
          this.MeasuredWidth = this.srcData.MeasuredWidth;
          break;

        case nameof(this.srcData.FeetPerMinute):
          this.Speed = (int)this.srcData.FeetPerMinute;
          break;
      }
    }

    private void SewinQueueChanged()
    {
      if (!this.sewinQueue.Rolls.Contains(this.CurrentRoll))
      {
        this.CurrentRoll = this.sewinQueue.Rolls.FirstOrDefault() ?? this.CurrentRoll;
      }
    }

    private void FeetCounterChanged(int feetCounter)
    {
      int measuredLength = feetCounter - this.feetCounterAtRollStart;

      try
      {
        if (this.seamAckNeeded && this.srcData.FeetCounter - this.feetCounterAtLastSeam >= this.appInfo.MinSeamSpacing)
        {
          this.seamAckNeeded = false;
          this.srcData.AcknowledgeSeamDetect();
        }

        if (this.CurrentRoll == null)
        {
          return;
        }

        //meter reset has been initiated but not completed, ignore this value
        if (measuredLength == 0)
        {
          return;
        }

        //if (this.meterResetAtLength > 0 && measuredLength - this.meterResetAtLength >= 0 && measuredLength - this.meterResetAtLength < 10)
        //{
        //  return;
        //}

        //this.meterResetAtLength = 0;

        //update roll info
        this.MeasuredLength = measuredLength;
        //oCurrentRoll.MeasuredWidth = oMahlo.RollWidth;

        //update display values
        this.Simulate_lblMeasuredLen_TextChanged();

        //lblLineSpeed.Text = System.Math.Round(oMahlo.LineSpeed, 1).ToString();
        //lblMeasuredWidth.Text = System.Math.Round(oMahlo.RollWidth, nWidthDisplayDecimalPlaces).ToString();
        //lblMeasuredLen.Text = System.Math.Round(Value, nLengthDisplayDecimalPlaces).ToString();

        //check for roll too long condition (possible missed seam)
        if (this.checkRollSize && (this.appInfo.MinRollLengthForLengthChecking == 0 || this.CurrentRoll.RollLength >= this.appInfo.MinRollLengthForLengthChecking))
        {
          if (this.MeasuredLength > (this.CurrentRoll.RollLength * this.appInfo.RollTooLongThreshold))
          {
            this.checkRollSize = false;
            this.IsMapValid = false;
            this.IsMappingNow = false;
            this.UserAttentions.IsRollTooLong = true;
          }
        }

        //save realtime data to dataset
        if (this.IsMappingNow)
        {
          SaveRealtimeDataToDataSet();
        }

        //if (oMahlo.LineSpeed == 0 || Math.Abs(nLastLineSpeed - oMahlo.LineSpeed) > 1)
        //{
        //  SaveLineSpeedToFile();
        //}
      }
      catch (Exception ex)
      {
        //WriteToErrorLog(ex.TargetSite.ToString(), ex.Message);
      }

      return;
    }

    private bool seamAckNeeded;
    private double feetCounterAtLastSeam;
    private void SeamDetected(bool isSeamDetected)
    {
      if (!isSeamDetected)
      {
        return;
      }

      double priorFeetCounter = this.feetCounterAtLastSeam;
      this.seamAckNeeded = true;
      this.feetCounterAtLastSeam = this.srcData.FeetCounter;

      //ignore seam detect if system is disabled
      if (this.UserAttentions.IsSystemDisabled)
      {
        return;
      }

      if (this.CurrentRoll.IsCheckRoll && this.srcData.FeetCounter - priorFeetCounter > this.appInfo.MaxEndCheckRollPieceLength)
      {
        return;
      }

      if (this.MeasuredLength < this.appInfo.MinSeamSpacing)
      {
        return;
      }

      try
      {
        //lock the queue
        //oSewinQueue.Locked = true;

        //store current roll measured length into variable in case it is needed
        //below to check for roll too short condition
        double rollExpectedLength = 0.0;
        double rollShortMeasuredLength = 0.0;
        if (this.checkRollSize && (this.appInfo.MinRollLengthForLengthChecking == 0 || this.CurrentRoll.RollLength >= this.appInfo.MinRollLengthForLengthChecking))
        {
          rollShortMeasuredLength = this.MeasuredLength;
          rollExpectedLength = this.CurrentRoll.RollLength;
        }

        //if current map is valid, store it permanently
        if (this.IsMapValid)
        {
          SaveRollMap();
        }

        this.OnRollFinished(this.CurrentRoll);

        string prevStyle = this.CurrentRoll.StyleCode;

        //must reset mahlo footage counter immediately so roll measurement is as
        //accurate as possible
        this.IsSeamDetected = true;
        //this.meterResetAtLength = this.MeasuredLength;
        this.feetCounterAtRollStart = (int)this.srcData.FeetCounter;

        //set seam detect indicator on immediately
        this.srcData.SetMiscellaneousIndicator(true);

        int index = this.sewinQueue.Rolls.IndexOf(this.CurrentRoll);
        if (index < this.sewinQueue.Rolls.Count - 1)
        {
          //advance to next position in queue
          this.CurrentRoll = this.sewinQueue.Rolls[index + 1];
          this.MeasuredLength = 0;
          //InitializeRollsFromQueue(oCurrentRoll.PositionInQueue + 1);
          //InitializeCharts();)
          switch (CommonMethods.DetermineRollType(this.sewinQueue.Rolls, this.CurrentRoll))
          {
            case CarpetRollTypeEnum.Greige:
              //automatically set recipe
              ApplyRecipe(this.CurrentRoll.DefaultRecipe, false);

              //check roll and style counts, alarm if necessary
              if (this.appInfo.MinRollLengthForStyleAndRollCounting > 0 && this.CurrentRoll.RollLength >= this.appInfo.MinRollLengthForStyleAndRollCounting)
              {
                this.rollCount++;
                if (prevStyle != this.CurrentRoll.StyleCode)
                {
                  this.styleCount++;
                }

                if (this.rollCount > this.appInfo.CheckAfterHowManyRolls || this.styleCount > this.appInfo.CheckAfterHowManyStyles)
                {
                  this.UserAttentions.VerifyRollSequence = true;
                }

                if (!this.UserAttentions.Any && !this.CriticalStops.Any)
                {
                  this.IsMapValid = true;
                  this.IsMappingNow = true;
                }
                else if (this.IsMappingNow && this.UserAttentions.Any)
                {
                  //WriteToErrorLog("SeamDetected", "Mapping stopped due to seam detect with user alerts present.");
                  this.IsMapValid = false;
                  this.IsMappingNow = false;
                }
              }

              break;

            default:
              //set controller to manual
              ApplyRecipe(string.Empty, true);
              this.IsMapValid = false;
              this.IsMappingNow = false;
              break;
          }

          //check for roll too short condition (possible queue positioning condition)
          //this must occur after counter is reset and roll repositioned within queue
          if (rollShortMeasuredLength > 0 && rollShortMeasuredLength < (rollExpectedLength * this.appInfo.RollTooShortThreshold))
          {
            this.checkRollSize = false;
            this.UserAttentions.IsRollTooShort = true;
          }
          else
          {
            this.checkRollSize = this.IsMappingNow;
          }
        }
        else
        {
          this.IsMappingNow = false;
        }

        this.OnRollStarted(this.CurrentRoll);
      }
      catch (Exception ex)
      {
        //WriteToErrorLog(ex.TargetSite.ToString(), ex.Message);
      }
    }

    public void RefreshStatusDisplay()
    {
      throw new NotImplementedException();
    }

    public void MoveToNextRoll()
    {
      this.IsSeamDetected = false;
      this.IsMappingNow = false;
      int index = this.sewinQueue.Rolls.IndexOf(this.CurrentRoll);
      if (index >= 0 && index < this.sewinQueue.Rolls.Count - 1)
      {
        this.CurrentRoll = this.sewinQueue.Rolls[index + 1];
      }
    }

    public void MoveToPriorRoll()
    {
      this.IsSeamDetected = false;
      this.IsMappingNow = false;
      int index = this.sewinQueue.Rolls.IndexOf(this.CurrentRoll);
      if (index > 0)
      {
        this.CurrentRoll = this.sewinQueue.Rolls[index - 1];
      }
    }

    public void WaitForSeam()
    {
      // pressing this button will essentially "re-arm" the system, regardless of it's
      // current state

      // reset roll and style counters
      this.rollCount = 1;
      this.styleCount = 1;

      // clear all user alerts
      this.UserAttentions.ClearAll();
    }

    public void DisableSystem()
    {
      // system is already disabled
      if (this.UserAttentions.IsSystemDisabled)
      {
        return;
      }

      // invalidate mapping for current roll
      this.IsMapValid = false;
      this.IsMappingNow = false;

      //WriteToErrorLog("DisableSystem", "Mapping stopped due to system being disabled.");

      this.UserAttentions.IsSystemDisabled = true;
    }
  }
}
