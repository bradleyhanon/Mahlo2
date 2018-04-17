using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
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
    private dynamic programState;

    IDisposable feetCounterSubscription;
    IDisposable seamDetectedSubscription;

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
    }

    private void RestoreState()
    {
      dynamic state = this.programState[nameof(MeterLogic<Model>)];
      //int currentRollId = state[nameof(currentRollId)];
    }

    /// <summary>
    /// Gets the current greige roll.
    /// </summary>
    public GreigeRoll CurrentGreigeRoll { get; private set; }

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

    public void Dispose()
    {
      this.feetCounterSubscription?.Dispose();
      this.seamDetectedSubscription?.Dispose();
    }

    /// <summary>
    /// Called to start data collection
    /// </summary>
    public void Start()
    {
      this.feetCounterSubscription = srcData.FeetCounter.Subscribe(value => this.FeetCounterChanged(value));
      this.seamDetectedSubscription = srcData.SeamDetected.Subscribe(value => this.SeamDetected(value));
    }

    private void FeetCounterChanged(int feet)
    {
      this.CurrentRoll.Feet = feet;
    }

    private void SeamDetected(bool isSeamDetected)
    {
      if (!isSeamDetected)
      {
        return;
      }
    }
  }
}
