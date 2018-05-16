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
using Mahlo;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;
using Mahlo.Utilities;
using Newtonsoft.Json;
using PropertyChanged;

namespace Mahlo.Logic
{
  [AddINotifyPropertyChangedInterface]
  abstract class MeterLogic<Model> : IMeterLogic<Model>, IDisposable
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
    private IDisposable userAttentionsChangedSubscription;
    private IDisposable critialErrorsChangedSubscription;

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
      this.UserAttentions = userAttentions;
      this.CriticalStops = criticalStops;
      this.programState = programState;
      this.RestoreState();
      this.feetCounterSubscription = srcData.FeetCounter.Subscribe(value => this.FeetCounterChanged(value));
      this.seamDetectedSubscription = srcData.SeamDetected.Subscribe(value => this.SeamDetected(value));
      this.feetPerMinuteSubscription = srcData.FeetPerMinute.Subscribe(value => this.Speed = value);
      this.queueChangedSubscription = sewinQueue.QueueChanged.Subscribe(_ => this.SewinQueueChanged());
      this.timer = Observable
        .Interval(TimeSpan.FromSeconds(1), schedulerProvider.WinFormsThread)
        .Subscribe(_ =>
        {
          this.Recipe = this.srcData.Recipe;
          this.IsManualMode = this.srcData.IsManualMode;
        });

      this.RollStarted = this.rollStartedSubject;
      this.RollFinished = this.rollFinishedSubject;

      this.userAttentionsChangedSubscription = Observable
        .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.UserAttentions).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.UserAttentions).PropertyChanged -= h)
        .Subscribe(_ => this.IsMapValid &= this.UserAttentions.Any);

      this.critialErrorsChangedSubscription = Observable
        .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.CriticalStops).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.CriticalStops).PropertyChanged -= h)
        .Subscribe(_ => this.IsMapValid &= this.CriticalStops.Any);

      Application.Idle += Application_Idle;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this object has been changed
    /// It is set by PropertyChangedFody and reset by Ipc.MahloServer
    /// </summary>
    public bool IsChanged { get; set; }

    /// <summary>
    /// Get the roll that is currently being processed
    /// </summary>
    public CarpetRoll CurrentRoll { get; set; } = new CarpetRoll();

    [DependsOn(nameof(CurrentRoll))]
    public string CurrentRollNo => this.CurrentRoll.RollNo;

    public IUserAttentions UserAttentions { get; set; }

    public ICriticalStops CriticalStops { get; set; }

    public IObservable<CarpetRoll> RollStarted { get; }
    public IObservable<CarpetRoll> RollFinished { get; }

    public abstract int Feet { get; set; }
    public abstract int Speed { get; set; }
    public abstract bool IsMapValid { get; set; }

    public bool IsManualMode { get; set; }

    public string Recipe { get; set; }

    public int PreviousRollLength { get; set; }

    public int RollChangesUntilCheckRequired
    {
      get => Math.Max(0, this.appInfo.CheckAfterHowManyRolls - this.rollCheckCount);
      set => throw new NotImplementedException();
    }

    public int StyleChangesUntilCheckRequired
    {
      get => Math.Max(0, this.appInfo.CheckAfterHowManyStyles - this.styleCheckCount);
      set => throw new NotImplementedException();
    }
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
      this.rollCheckCount = state.Get<int?>(nameof(rollCheckCount)) ?? 0;
      this.styleCheckCount = state.Get<int?>(nameof(styleCheckCount)) ?? this.styleCheckCount;
      this.CurrentRoll = state?.Get<CarpetRoll>(nameof(CurrentRoll)) ?? new CarpetRoll();
      this.sewinQueue.TryGetRoll(this.CurrentRoll.Id, out CarpetRoll roll);
      this.CurrentRoll = roll;

      // On startup, roll sequence should be verified
      this.UserAttentions.VerifyRollSequence = true;
    }

    protected virtual void Dispose(bool disposing)
    {
      // Save program state
      var state = programState.GetSubState(nameof(MeterLogic<Model>));
      state.Set(typeof(Model).Name, new
      {
        this.rollCheckCount,
        this.styleCheckCount,
        this.CurrentRoll,
      });

      this.feetCounterSubscription?.Dispose();
      this.seamDetectedSubscription?.Dispose();
      this.feetPerMinuteSubscription?.Dispose();
      this.queueChangedSubscription?.Dispose();
      this.timer.Dispose();
      this.userAttentionsChangedSubscription?.Dispose();
      this.critialErrorsChangedSubscription?.Dispose();
    }

    private void Application_Idle(object sender, EventArgs e)
    {
      
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
      this.UserAttentions.IsRollTooLong |= this.Feet > this.CurrentRoll.RollLength * 1.1;
    }

    private void SeamDetected(bool isSeamDetected)
    {
      if (!isSeamDetected)
      {
        return;
      }

      this.srcData.ResetSeamDetector();

      if (this.UserAttentions.IsSystemDisabled)
      {
        // Do not respond to seam detection if system is disabled
        return;
      }

      if (this.Feet < this.appInfo.SeamDetectableThreshold)
      {
        // Do not respond to seam if footage is below threshold, could be detecting same seam
        return;
      }

      this.UserAttentions.IsRollTooShort |= this.Feet < this.CurrentRoll.RollLength * 0.9;
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
        this.UserAttentions.VerifyRollSequence = true;
      }

      this.CurrentRoll = nextRoll;

      this.Feet = 0;
      this.IsMapValid = this.GetIsMappingValid();
      this.srcData.ResetMeterOffset();

      this.rollStartedSubject.OnNext(this.CurrentRoll);
    }

    public void RefreshStatusDisplay()
    {
      throw new NotImplementedException();
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
