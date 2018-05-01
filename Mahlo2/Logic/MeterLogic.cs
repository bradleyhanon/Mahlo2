using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Mahlo;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;
using Mahlo.Utilities;
using PropertyChanged;

namespace Mahlo.Logic
{
  [AddINotifyPropertyChangedInterface]
  abstract class MeterLogic<Model> : IMeterLogic<Model>, IModelLogic
    where Model : MahloRoll, new()
  {
    private ISewinQueue sewinQueue;
    private IMeterSrc<Model> srcData;
    private IAppInfoBAS appInfo;
    private IProgramState programState;

    private Subject<CarpetRoll> rollFinishedSubject = new Subject<CarpetRoll>();
    private Subject<CarpetRoll> rollStartedSubject = new Subject<CarpetRoll>();
    private IDisposable feetCounterSubscription;
    private IDisposable seamDetectedSubscription;
    private IDisposable feetPerMinuteSubscription;
    private IDisposable queueChangedSubscription;
    private IDisposable timer;

    private int rollCheckCount;
    private int styleCheckCount = 1;

    public MeterLogic(
      IMeterSrc<Model> srcData, 
      ISewinQueue sewinQueue,
      IAppInfoBAS appInfo,
      IUserAttentions<Model> userAttentions, 
      ICriticalStops<Model> criticalStops,
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
    {
      this.sewinQueue = sewinQueue;
      this.srcData = srcData;
      this.appInfo = appInfo;
      this.UserAttentsions = userAttentions;
      this.CriticalStops = criticalStops;
      this.programState = programState;
      this.RestoreState();
      this.feetCounterSubscription = srcData.FeetCounter.Subscribe(value => this.FeetCounterChanged(value));
      this.seamDetectedSubscription = srcData.SeamDetected.Subscribe(value => this.SeamDetected(value));
      this.feetPerMinuteSubscription = srcData.FeetPerMinute.Subscribe(value => this.Speed = value);
      this.queueChangedSubscription = sewinQueue.QueueChanged.Subscribe(_ => this.SewinQueueChanged());
      this.timer = Observable
        .Interval(TimeSpan.FromSeconds(1), schedulerProvider.WinFormsThread)
        .Subscribe(_ => this.RefreshStatusDisplay());

      this.RollStarted = this.rollStartedSubject;
      this.RollFinished = this.rollFinishedSubject;
    }

    /// <summary>
    /// Get the roll that is currently being processed
    /// </summary>
    public CarpetRoll CurrentRoll { get; set; } = new CarpetRoll();

    public IUserAttentions<Model> UserAttentsions { get; }

    public ICriticalStops<Model> CriticalStops { get; }

    public IObservable<CarpetRoll> RollStarted { get; }
    public IObservable<CarpetRoll> RollFinished { get; }

    public string MahloStatusMessage { get; private set; }
    public Color MahloStatusMessageBackColor { get; private set; }
    [DependsOn(nameof(MahloStatusMessageBackColor))]
    public Color MahloStatusMessageForeColor => MahloStatusMessageBackColor.ContrastColor();

    public string PlcStatusMessage { get; private set; }
    public Color PlcStatusMessageBackColor { get; private set; }
    [DependsOn(nameof(PlcStatusMessageBackColor))]
    public Color PlcStatusMessageForeColor => PlcStatusMessageBackColor.ContrastColor();

    public string MappingStatusMessage { get; private set; }
    public Color MappingStatusMessageBackColor { get; private set; }
    [DependsOn(nameof(MappingStatusMessageBackColor))]
    public Color MappingStatusMessageForeColor => MappingStatusMessageBackColor.ContrastColor();


    public bool IsMappingValid => !this.CurrentRoll.IsCheckRoll && !this.UserAttentsions.Any && !this.CriticalStops.Any;

    public abstract int Feet { get; set; }
    public abstract int Speed { get; set; }

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

    protected virtual void RestoreState()
    {
      var state = this.programState.GetSubState(nameof(MeterLogic<Model>), nameof(Model));
      this.rollCheckCount = state.Get<int?>(nameof(rollCheckCount)) ?? 0;
      this.styleCheckCount = state.Get<int?>(nameof(styleCheckCount)) ?? this.styleCheckCount;
      this.CurrentRoll = state?.Get<CarpetRoll>(nameof(CurrentRoll)) ?? new CarpetRoll();
      this.sewinQueue.TryGetRoll(this.CurrentRoll.Id, out CarpetRoll roll);
      this.CurrentRoll = roll;

      // On startup, roll sequence should be verified
      this.UserAttentsions.VerifyRollSequence = true;
    }

    protected virtual void Dispose(bool disposing)
    {
      var state = programState.GetSubState(nameof(MeterLogic<Model>));
      state.Set(typeof(Model).Name, new
      {
        this.rollCheckCount,
        this.styleCheckCount,
        this.CurrentRoll,
      });

      this.timer.Dispose();
      this.feetCounterSubscription?.Dispose();
      this.seamDetectedSubscription?.Dispose();
      this.feetPerMinuteSubscription?.Dispose();
    }

    private void SewinQueueChanged()
    {
      if (!this.sewinQueue.Rolls.Contains( this.CurrentRoll))
      {
        this.CurrentRoll = this.sewinQueue.Rolls.FirstOrDefault() ?? this.CurrentRoll;
      }
    }

    private void FeetCounterChanged(int feet)
    {
      this.Feet = feet;
      this.UserAttentsions.IsRollTooLong |= this.Feet > this.CurrentRoll.RollLength * 1.1;
    }

    private void SeamDetected(bool isSeamDetected)
    {
      if (!isSeamDetected)
      {
        return;
      }

      this.srcData.ResetSeamDetector();

      if (this.UserAttentsions.IsSystemDisabled)
      {
        // Do not respond to seam detection if system is disabled
        return;
      }

      if (this.Feet < this.appInfo.SeamDetectableThreshold)
      {
        // Do not respond to seam if footage is below threshold, could be detecting same seam
        return;
      }

      this.UserAttentsions.IsRollTooShort |= this.Feet < this.CurrentRoll.RollLength * 0.9;
      this.rollFinishedSubject.OnNext(this.CurrentRoll);

      // Start new roll
      this.rollCheckCount++;
      this.sewinQueue.TryGetRoll(this.CurrentRoll.Id + 1, out CarpetRoll nextRoll);

      if (this.CurrentRoll.StyleCode != nextRoll.StyleCode)
      {
        styleCheckCount++;
      }

      if (this.rollCheckCount >= this.appInfo.CheckAfterHowManyRolls || 
        this.styleCheckCount >= this.appInfo.CheckAfterHowManyStyles)
      {
        this.UserAttentsions.VerifyRollSequence = true;
      }

      this.CurrentRoll = nextRoll;
      this.Feet = 0;
      this.srcData.ResetMeterOffset();

      this.rollStartedSubject.OnNext(this.CurrentRoll);
    }

    private void RefreshStatusDisplay()
    {
      // Mahlo status
      (Color backColor, string message) =
        this.CriticalStops.IsMahloCommError ? (Color.Red, "Mahlo is NOT communicating") :
        this.srcData.IsManualMode ? (Color.Yellow, "Mahlo is in Manual mode") :
        (Color.Green, $"Mahlo Recipe: {(string.IsNullOrWhiteSpace(srcData.Recipe) ? "Unknown" : srcData.Recipe)}");
      this.MahloStatusMessage = message;
      this.MahloStatusMessageBackColor = backColor;

      // PLC status
      (backColor, message) = 
        this.CriticalStops.IsPlcCommError ? (Color.Red, "PLC is NOT Communicating") :
        (Color.Green, "PLC is Communicating");
      this.PlcStatusMessage = message;
      this.PlcStatusMessageBackColor = backColor;

      // Mapping status
      (backColor, message) =
        this.CriticalStops.Any ? (Color.Red, "Mapping is off due to one or more critical problems") :
        this.UserAttentsions.IsSystemDisabled ? (Color.Yellow, "System is disabled, seams are ignored, press [F3] to arm") :
        this.UserAttentsions.IsRollTooLong ? (Color.Yellow, "Measured roll length excessively long, verify roll sequence") :
        this.UserAttentsions.IsRollTooShort ? (Color.Yellow, "Measured roll length excessively short, verify roll sequence") :
        this.UserAttentsions.VerifyRollSequence ? (Color.Yellow, "Verify roll sequence, click \"Wait for Seam\" to arm system") :
        (Color.Green, "Roll is being mapped");
      this.MappingStatusMessage = message;
      this.MappingStatusMessageBackColor = backColor;
    }

    public void MoveToNextRoll()
    {
      int index = this.sewinQueue.Rolls.IndexOf(this.CurrentRoll);
      if (index >= 0 && index < this.sewinQueue.Rolls.Count - 1)
      {
        this.CurrentRoll = this.sewinQueue.Rolls[index + 1];
      }
    }

    public void MoveToPriorRoll()
    {
      int index = this.sewinQueue.Rolls.IndexOf(this.CurrentRoll);
      if (index > 0)
      {
        this.CurrentRoll = this.sewinQueue.Rolls[index - 1];
      }
    }

    public void WaitForSeam()
    {
      //throw new NotImplementedException();
    }
  }
}
