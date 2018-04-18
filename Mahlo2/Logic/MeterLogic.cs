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

namespace Mahlo.Logic
{
  sealed class MeterLogic<Model> : IMeterLogic<Model>
    where Model : MahloRoll, new()
  {
    private ISewinQueue sewinQueue;
    private IMeterSrc<Model> srcData;
    private IAppInfoBAS appInfo;
    private IProgramState programState;

    private Subject<Model> rollFinishedSubject = new Subject<Model>();
    private Subject<Model> rollStartedSubject = new Subject<Model>();
    private IDisposable feetCounterSubscription;
    private IDisposable seamDetectedSubscription;

    private int rollCheckCount;
    private int styleCheckCount = 1;

    public MeterLogic(
      IMeterSrc<Model> srcData, 
      ISewinQueue sewinQueue,
      IAppInfoBAS appInfo,
      IUserAttentions<Model> userAttentions, 
      ICriticalStops<Model> criticalStops,
      IProgramState programState)
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

      this.RollStarted = this.rollStartedSubject;
      this.RollFinished = this.rollFinishedSubject;
    }

    /// <summary>
    /// Gets the current greige roll.
    /// </summary>
    public GreigeRoll CurrentGreigeRoll { get; set; }

    /// <summary>
    /// Get the roll that is currently being processed
    /// </summary>
    public Model CurrentRoll { get; private set; } = new Model();

    public int CurrentRollId { get; private set; }

    public IUserAttentions<Model> UserAttentsions { get; }

    public ICriticalStops<Model> CriticalStops { get; }

    public IObservable<Model> RollStarted { get; }
    public IObservable<Model> RollFinished { get; }

    public bool IsMappingValid => !this.CurrentGreigeRoll.IsCheckRoll && !this.UserAttentsions.Any && !this.CriticalStops.Any;


    /// <summary>
    /// Called to start data collection
    /// </summary>
    public void Start()
    {
    }

    private void RestoreState()
    {
      var state = this.programState.GetSubState(nameof(MeterLogic<Model>), nameof(Model));
      this.rollCheckCount = state.Get<int?>(nameof(rollCheckCount)) ?? 0;
      this.styleCheckCount = state.Get<int?>(nameof(styleCheckCount)) ?? this.styleCheckCount;
      this.CurrentRoll = state?.Get<Model>(nameof(CurrentRoll)) ?? new Model();
      this.sewinQueue.TryGetRoll(this.CurrentRoll.RollId, out GreigeRoll roll);
      this.CurrentGreigeRoll = roll;

      // On startup, roll sequence should be verified
      this.UserAttentsions.VerifyRollSequence = true;
    }

    public void Dispose()
    {
      var state = programState.GetSubState(nameof(MeterLogic<Model>));
      state.Set(typeof(Model).Name, new
      {
        this.rollCheckCount,
        this.styleCheckCount,
        this.CurrentRoll,
      });

      this.feetCounterSubscription?.Dispose();
      this.seamDetectedSubscription?.Dispose();
    }

    private void FeetCounterChanged(int feet)
    {
      this.CurrentRoll.Feet = feet;
      this.UserAttentsions.IsRollTooLong |= this.CurrentRoll.Feet > this.CurrentGreigeRoll.RollLength * 1.1;
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

      if (this.CurrentRoll.Feet < this.appInfo.SeamDetectableThreshold)
      {
        // Do not respond to seam if footage is below threshold, could be detecting same seam
        return;
      }

      this.UserAttentsions.IsRollTooShort |= this.CurrentRoll.Feet < this.CurrentGreigeRoll.RollLength * 0.9;
      this.rollFinishedSubject.OnNext(this.CurrentRoll);
      this.srcData.ResetMeterOffset();

      // Start new roll
      this.CurrentRoll.Feet = 0;
      this.rollCheckCount++;
      this.sewinQueue.TryGetRoll(this.CurrentGreigeRoll.RollId + 1, out GreigeRoll nextGreigeRoll);
      if (this.CurrentGreigeRoll.StyleCode != nextGreigeRoll.StyleCode)
      {
        styleCheckCount++;
      }

      if (this.rollCheckCount >= this.appInfo.CheckAfterHowManyRolls || 
        this.styleCheckCount >= this.appInfo.CheckAfterHowManyStyles)
      {
        this.UserAttentsions.VerifyRollSequence = true;
      }

      this.CurrentGreigeRoll = nextGreigeRoll;
      this.rollStartedSubject.OnNext(this.CurrentRoll);
    }
  }
}
