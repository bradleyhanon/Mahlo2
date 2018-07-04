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
  class PatternRepeatLogic : MeterLogic<PatternRepeatModel>, IPatternRepeatLogic
  {
    readonly IServiceSettings appInfo;
    private IPatternRepeatSrc srcData;
    private CutRollList cutRolls;
    private IDbLocal dbLocal;

    private double maxElongation;
    private long feetCounterAtCutRollStart;
    private int nextCutRollId;
    private CancellationTokenSource ctsDoff;
    private bool isFeetCounterChanged;
    private readonly PatternRepeatMapDatum mapDatum = new PatternRepeatMapDatum();

    public PatternRepeatLogic(
      IDbLocal dbLocal,
      CutRollList cutRolls,
      IPatternRepeatSrc srcData,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<PatternRepeatModel> userAttentions,
      ICriticalStops<PatternRepeatModel> criticalStops,
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
          this.CurrentCutRoll = this.cutRolls.LastOrDefault();
        }));
    }

    public override GreigeRoll CurrentRoll
    {
      get => base.CurrentRoll;
      set
      {
        base.CurrentRoll = value;
        this.cutRolls.Clear();
        this.cutRolls.AddRange(this.dbLocal.GetCutRollsFor(this.CurrentRoll.Id));
      }
    }

    [JsonIgnore]
    public CutRoll CurrentCutRoll { get; set; }

    public bool IsDoffDetected { get; set; }

    public override long FeetCounterStart
    {
      get => this.CurrentRoll.PrsFeetCounterStart;
      set => this.CurrentRoll.PrsFeetCounterStart = value;
    }

    public override long FeetCounterEnd
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

    protected override string MapTableName => "PatternRepeatMap";

    protected override void SaveMapDatum()
    {
      this.mapDatum.FeetCounter = this.CurrentFeetCounter;
      this.dbLocal.InsertPatternRepeatMapDatum(this.mapDatum);
      this.mapDatum.Elongation = 0.0;

      // Update Current cut roll every time datum is recorded
      this.dbLocal.UpdateCutRoll(this.CurrentCutRoll);
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

      this.feetCounterAtCutRollStart = this.CurrentFeetCounter;
    }

    protected override void OpcValueChanged(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(this.srcData.PatternRepeatLength):
          if (this.IsMovementForward)
          {
            double elongation = srcData.PatternRepeatLength - this.CurrentRoll.PatternRepeatLength;
            if (Math.Abs(elongation) > Math.Abs(this.CurrentRoll.Elongation))
            {
              this.CurrentRoll.Elongation = elongation;
            }

            if (this.CurrentCutRoll != null && Math.Abs(elongation) > Math.Abs(this.CurrentCutRoll.MaxEPE))
            {
              this.CurrentCutRoll.MaxEPE = elongation;
            }
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

    protected override void FeetCounterChanged()
    {
      base.FeetCounterChanged();

      if (this.CurrentCutRoll == null)
      {
        return;
      }

      this.isFeetCounterChanged = this.CurrentCutRoll.FeetCounterEnd != this.CurrentFeetCounter;
      this.CurrentCutRoll.FeetCounterEnd = this.CurrentFeetCounter;
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
        // Doff signal gone away so cancel any further doff acks
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
          FeetCounterStart = this.CurrentFeetCounter,
          FeetCounterEnd = this.CurrentFeetCounter,
        };

        this.cutRolls.Add(this.CurrentCutRoll);
        this.dbLocal.AddCutRoll(this.CurrentCutRoll);
      }

      this.ctsDoff?.Dispose();
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
