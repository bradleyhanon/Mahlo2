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
using System.Reactive.Concurrency;

namespace MahloService.Logic
{
  class PatternRepeatLogic : MeterLogic<PatternRepeatModel>, IPatternRepeatLogic
  {
    readonly IServiceSettings serviceSettings;
    private readonly IPatternRepeatSrc srcData;
    private CutRollList cutRolls;
    private readonly ISapRollAssigner sapRollAssigner;
    private readonly IDbLocal dbLocal;

    private readonly Averager currentRollAverager = new Averager();
    private readonly Averager mapDatumAverager = new Averager();
    private long feetCounterAtCutRollStart;
    private int nextCutRollId;
    private CancellationTokenSource ctsDoff;
    private bool isFeetCounterChanged;
    private readonly PatternRepeatMapDatum mapDatum = new PatternRepeatMapDatum();
    private readonly List<double> cutRollElongations = new List<double>();

    public PatternRepeatLogic(
      IDbLocal dbLocal,
      CutRollList cutRolls,
      ISapRollAssigner sapRollAssigner,
      IPatternRepeatSrc srcData,
      ISewinQueue sewinQueue,
      IServiceSettings serviceSettings,
      IUserAttentions<PatternRepeatModel> userAttentions,
      ICriticalStops<PatternRepeatModel> criticalStops,
      IProgramState programState,
      IScheduler scheduler)
      : base(dbLocal, srcData, sewinQueue, serviceSettings, userAttentions, criticalStops, programState, scheduler)
    {
      this.dbLocal = dbLocal;
      this.cutRolls = cutRolls;
      this.sapRollAssigner = sapRollAssigner;
      this.serviceSettings = serviceSettings;
      this.srcData = srcData;

      this.nextCutRollId = dbLocal.GetNextCutRollId();

      this.SeamDelayLine.DelayTicks = serviceSettings.SeamToCutKnife;

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

      this.KeepBowAndSkewUpToDate();
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

    public double Elongation
    {
      get => this.CurrentRoll.Elongation;
      set => this.CurrentRoll.Elongation = value;
    }

    protected override string MapTableName => "PatternRepeatMap";

    protected override GreigeRoll FindCurrentRollOnStartup(ISewinQueue sewinQueue)
    {
      return sewinQueue.Rolls.LastOrDefault(roll => roll.PrsFeetCounterEnd != 0);
    }

    protected override void SaveMapDatum()
    {
      if (this.mapDatumAverager.Count > 0)
      {
        this.mapDatum.FeetCounter = this.CurrentFeetCounter;
        this.mapDatum.Elongation = this.mapDatumAverager.Average;
        this.dbLocal.InsertPatternRepeatMapDatum(this.mapDatum);
      }

      this.cutRollElongations.Add(this.mapDatum.Elongation);

      this.mapDatumAverager.Clear();

      // Update Current cut roll every time datum is recorded
      if (this.CurrentCutRoll != null)
      {
        this.CurrentCutRoll.EPE =
          CalculateEPE(this.CurrentRoll.PatternRepeatLength, this.cutRollElongations);
        this.CurrentCutRoll.Dlot =
          CalculateDlot(this.CurrentRoll.BackingCode, this.CurrentCutRoll.EPE, this.serviceSettings);

        this.dbLocal.UpdateCutRoll(this.CurrentCutRoll);
      }
    }

    protected override void OnRollFinished(GreigeRoll greigeRoll)
    {
      base.OnRollFinished(greigeRoll);
      this.Elongation = this.currentRollAverager.Average;
    }

    protected override void OnRollStarted(GreigeRoll greigeRoll)
    {
      base.OnRollStarted(greigeRoll);
      this.currentRollAverager.Clear();

      this.feetCounterAtCutRollStart = this.CurrentFeetCounter;
    }

    protected override void OpcValueChanged(string propertyName)
    {
      base.OpcValueChanged(propertyName);

      switch (propertyName)
      {
        case nameof(this.srcData.FeetCounter):
          if (this.IsMovementForward)
          {
            this.currentRollAverager.Add(this.srcData.PatternRepeatLength);
            this.mapDatumAverager.Add(this.srcData.PatternRepeatLength);
          }

          break;

        case nameof(this.srcData.PatternRepeatLength):
          if (this.IsMovementForward)
          {
            this.Elongation = this.srcData.PatternRepeatLength; ;
          }

          break;

        case nameof(this.srcData.IsDoffDetected):
          this.DoffDetected();
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
            (this.CurrentCutRoll.Bow, this.CurrentCutRoll.Skew) =
              this.dbLocal.GetAverageBowAndSkew(this.CurrentCutRoll.GreigeRollId, this.CurrentCutRoll.FeetCounterStart, this.CurrentCutRoll.FeetCounterEnd);
          }
        }
      }
    }

    internal static double CalculateEPE(double patternRepeatLength, IEnumerable<double> elongations)
    {
      var spe = patternRepeatLength;
      var ape = elongations.Average();
      var epe = spe == 0.0 ? 1.0 : ape / spe;
      return epe;
    }

    internal static string CalculateDlot(string backingCode, double epe, IServiceSettings settings)
    {
      var spec = settings.GetBackingSpec(backingCode).DlotSpec;
      var a = ((Math.Abs(1.0 - epe) + (spec / 2)) / spec);
      var index = (int)Math.Min(5, a);
      return (index == 0 ? "" : epe < 1.0 ? "+" : "-") + index.ToString();
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
        this.sapRollAssigner.AssignSapRollTo(this.CurrentCutRoll);
      }

      // CurrentRoll.Id == 0 implies that there is no CurrentRoll that will be recorded 
      if (this.CurrentRoll.Id != 0 &&
        (this.CurrentCutRoll == null || this.CurrentCutRoll.Length >= this.serviceSettings.MinSeamSpacing))
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
        this.cutRollElongations.Clear();
      }

      this.ctsDoff?.Dispose();
      this.ctsDoff = new CancellationTokenSource();
      try
      {
        do
        {
          await Task.Delay(1000, this.ctsDoff.Token);
          this.srcData.AcknowledgeDoffDetect();
        } while (this.srcData.IsDoffDetected);
      }
      catch (TaskCanceledException)
      {
      }
    }
  }
}
