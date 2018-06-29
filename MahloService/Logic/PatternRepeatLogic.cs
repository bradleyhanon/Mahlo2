using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Utilities;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Collections.Specialized;
using System.Threading;

namespace MahloService.Logic
{
  class PatternRepeatLogic : MeterLogic<PatternRepeatRoll>, IPatternRepeatLogic
  {
    IServiceSettings appInfo;
    private IPatternRepeatSrc srcData;
    private ICutRollList cutRolls;
    private IDbLocal dbLocal;

    private double maxElongation;
    private int feetCounterAtCutRollStart;
    private int nextCutRollId;
    private CancellationTokenSource ctsDoff;
    private bool isFeetCounterChanged;

    public PatternRepeatLogic(
      IDbLocal dbLocal,
      ICutRollList cutRolls,
      IPatternRepeatSrc srcData,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<PatternRepeatRoll> userAttentions,
      ICriticalStops<PatternRepeatRoll> criticalStops,
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
      : base(dbLocal, srcData, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
    {
      this.dbLocal = dbLocal;
      this.cutRolls = cutRolls;
      this.appInfo = appInfo;
      this.srcData = srcData;

      this.nextCutRollId = dbLocal.GetNextCutRollId();

      this.Disposables.Add(
        Observable
        .FromEventPattern<ListChangedEventHandler, ListChangedEventArgs>(
          h => this.cutRolls.ListChanged += h,
          h => this.cutRolls.ListChanged -= h)
        .Where(args => args.EventArgs.ListChangedType != ListChangedType.ItemChanged)
        .Subscribe(_ =>
        {
          this.CurrentCutRoll = this.cutRolls.Last();
        }));
    }

    [JsonIgnore]
    public CutRoll CurrentCutRoll { get; set; }

    public bool IsDoffDetected { get; set; }

    public override int FeetCounterStart
    {
      get => this.CurrentRoll.PrsFeetCounterStart;
      set => this.CurrentRoll.PrsFeetCounterStart = value;
    }

    public override int FeetCounterEnd
    {
      get => this.CurrentRoll.PrsFeetCounterEnd;
      set => this.CurrentRoll.PrsFeetCounterEnd = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.PrsSpeed;
      set => this.CurrentRoll.PrsSpeed = value;
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.PrsMapValid;
      set => this.CurrentRoll.PrsMapValid = value;
    }

    protected override void OnRollFinished(GreigeRoll greigeRoll)
    {
      base.OnRollFinished(greigeRoll);
      greigeRoll.Elongation = this.maxElongation;
    }

    protected override void OnRollStarted(GreigeRoll greigeRoll)
    {
      base.OnRollStarted(greigeRoll);
      this.maxElongation = 0;

      this.feetCounterAtCutRollStart = (int)this.srcData.FeetCounter;
    }

    protected override void OpcValueChanged(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(this.srcData.PatternRepeatLength):
          double elongation = srcData.PatternRepeatLength - this.CurrentRoll.PatternRepeatLength;
          if (Math.Abs(elongation) > Math.Abs(this.CurrentRoll.Elongation))
          {
            this.CurrentRoll.Elongation = elongation;
          }

          if (Math.Abs(elongation) > Math.Abs(this.CurrentCutRoll.MaxEPE))
          {
            this.CurrentCutRoll.MaxEPE = elongation;
          }

          break;

        case nameof(this.srcData.IsDoffDetected):
          this.DoffDetected();
          break;

        default:
          base.OpcValueChanged(propertyName);
          break;
      }
    }

    protected override void FeetCounterChanged(int feetCounter)
    {
      base.FeetCounterChanged(feetCounter);

      if (this.CurrentCutRoll == null)
      {
        return;
      }

      this.isFeetCounterChanged = this.CurrentCutRoll.FeetCounterEnd != feetCounter;
      this.CurrentCutRoll.FeetCounterEnd = feetCounter;
    }

    private async void KeepBowAndSkewUpToDate()
    {
      for (; ;)
      {
        await Task.Delay(1000);
        if (this.isFeetCounterChanged)
        {
          this.isFeetCounterChanged = false;
          if (this.CurrentCutRoll != null)
          {
            (this.CurrentCutRoll.MaxBow, this.CurrentCutRoll.MaxSkew) = 
              dbLocal.GetBowAndSkew(this.CurrentCutRoll.GreigeRollId, this.CurrentCutRoll.FeetCounterStart, this.CurrentCutRoll.FeetCounterEnd);
          }
        }
      }
    }

    private async void DoffDetected()
    {
      if (!this.srcData.IsDoffDetected)
      {
        // Cancel any further doff acks
        this.ctsDoff?.Cancel();
        return;
      }

      if (this.CurrentCutRoll != null)
      {
        this.dbLocal.UpdateCutRoll(this.CurrentCutRoll);
      }

      if (this.CurrentCutRoll == null || this.CurrentCutRoll.Length >= this.appInfo.MinSeamSpacing)
      {
        // CutRoll is finished
        this.CurrentCutRoll = new CutRoll
        {
          Id = this.nextCutRollId++,
          GreigeRollId = this.CurrentRoll.Id,
          FeetCounterStart = (int)this.srcData.FeetCounter,
          FeetCounterEnd = (int)this.srcData.FeetCounter,
        };

        this.cutRolls.Add(this.CurrentCutRoll);
        this.dbLocal.AddCutRoll(this.CurrentCutRoll);
      }

      this.ctsDoff.Dispose();
      this.ctsDoff = new CancellationTokenSource();
      try
      {
        do
        {
          await Task.Delay(1000, ctsDoff.Token);
          this.srcData.AcknowledgeDoffDetect();
        } while (this.srcData.IsDoffDetected);
      }
      catch (TaskCanceledException)
      {
      }
    }
  }
}
