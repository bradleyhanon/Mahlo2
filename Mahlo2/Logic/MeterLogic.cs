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

namespace Mahlo.Logic
{
  abstract class MeterLogic<Model> : IMeterLogic<Model>
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
      this.timer = Observable
        .Interval(TimeSpan.FromSeconds(1), schedulerProvider.WinFormsThread)
        .Do(_ => Console.WriteLine("Timer Triggered!"))
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
    public string PlcStatusMessage { get; private set; }
    public string LogicStatusMessage { get; private set; }

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
      this.sewinQueue.TryGetRoll(this.CurrentRoll.RollId, out CarpetRoll roll);
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
      this.srcData.ResetMeterOffset();

      // Start new roll
      this.Feet = 0;
      this.rollCheckCount++;
      this.sewinQueue.TryGetRoll(this.CurrentRoll.RollId + 1, out CarpetRoll nextRoll);
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
      this.rollStartedSubject.OnNext(this.CurrentRoll);
    }

    private void RefreshStatusDisplay()
    {

      (Color backGround, Color foreGround, string Message) =
        this.CriticalStops.IsMahloCommError ? (Color.Red, Color.White, "Mahlo is NOT communicating") :
        (Color.Green, Color.Black, $"Mahlo Recipe: {(string.IsNullOrWhiteSpace(srcData.Recipe) ? "Unknown" : srcData.Recipe)}");


    }
  }
}
