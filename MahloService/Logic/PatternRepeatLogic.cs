using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Settings;
using MahloService.Utilities;
using Newtonsoft.Json;

namespace MahloService.Logic
{
  internal class PatternRepeatLogic : MeterLogic<PatternRepeatModel>, IPatternRepeatLogic
  {
    private readonly IServiceSettings serviceSettings;
    private readonly IPatternRepeatSrc srcData;
    private readonly ISewinQueue sewinQueue;
    private readonly CutRollList cutRolls;
    private readonly InspectionAreaList inspectionAreaList;
    private readonly ISapRollAssigner sapRollAssigner;
    private readonly IDbLocal dbLocal;
    private readonly IProgramState programState;

    private readonly Averager greigeRollAverager = new Averager();
    private readonly Averager mapDatumAverager = new Averager();
    private int nextCutRollId;
    private CancellationTokenSource ctsDoff;
    private bool isFeetCounterChanged;
    private readonly PatternRepeatMapDatum mapDatum = new PatternRepeatMapDatum();
    private readonly List<double> cutRollElongations = new List<double>();

    public PatternRepeatLogic(
      IDbLocal dbLocal,
      CutRollList cutRolls,
      InspectionAreaList inspectionAreaList,
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
      this.inspectionAreaList = inspectionAreaList;
      this.sapRollAssigner = sapRollAssigner;
      this.serviceSettings = serviceSettings;
      this.srcData = srcData;
      this.sewinQueue = sewinQueue;
      this.programState = programState;

      this.nextCutRollId = dbLocal.GetNextCutRollId();

      this.SeamDelayLine.DelayTicks = serviceSettings.SeamToCutKnife;

      programState.Saving += this.SaveState;

      this.KeepBowAndSkewUpToDateAsync().NoWait();
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
 
    protected override void RestoreState()
    {
      base.RestoreState();

      try
      {
        var state = this.programState.GetSubState(nameof(CutRoll));
        var id = state.Get<int?>(nameof(CutRoll.Id)) ?? 0;
        if (id != 0)
        {
          var cutRoll = this.cutRolls.FirstOrDefault(cr => cr.Id == id) ?? new CutRoll
          {
            Id = id,
          };

          cutRoll.GreigeRollId = state.Get<int>(nameof(CutRoll.GreigeRollId));
          cutRoll.SapRoll = state.Get<string>(nameof(CutRoll.SapRoll));
          cutRoll.FeetCounterStart = state.Get<int>(nameof(CutRoll.FeetCounterStart));
          cutRoll.FeetCounterEnd = state.Get<int>(nameof(CutRoll.FeetCounterEnd));
          cutRoll.Bow = state.Get<double>(nameof(CutRoll.Bow));
          cutRoll.Skew = state.Get<double>(nameof(CutRoll.Skew));
          cutRoll.EPE = state.Get<double>(nameof(CutRoll.EPE));
          cutRoll.Dlot = state.Get<string>(nameof(CutRoll.Dlot));
          cutRoll.Elongation = state.Get<double>(nameof(CutRoll.Elongation));

          this.CurrentCutRoll = cutRoll;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Unable to restore current cut roll");
        Console.WriteLine(ex.ToString());
      }
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
        this.CurrentCutRoll.Elongation =
          this.cutRollElongations.Average();

        this.dbLocal.UpdateCutRoll(this.CurrentCutRoll);
      }
    }

    protected override void OnRollFinished(GreigeRoll greigeRoll)
    {
      base.OnRollFinished(greigeRoll);
      this.Elongation = this.greigeRollAverager.Average;
    }

    protected override void OnRollStarted(GreigeRoll greigeRoll)
    {
      base.OnRollStarted(greigeRoll);
      this.greigeRollAverager.Clear();
      this.srcData.SetRecipeFromPatternLength(greigeRoll.PatternRepeatLength).NoWait();

      // If the current cut roll is too short, it belongs to the new greige roll rather than the prior one.
      if (this.CurrentCutRoll != null && this.CurrentCutRoll.Length < this.serviceSettings.MinSeamSpacing)
      {
        this.cutRolls.Add(this.CurrentCutRoll);
        this.CurrentCutRoll.GreigeRollId = greigeRoll.Id;
      }
    }

    protected override void OpcValueChanged(string propertyName)
    {
      base.OpcValueChanged(propertyName);

      switch (propertyName)
      {
        case nameof(this.srcData.FeetCounter):
          if (this.IsMovementForward)
          {
            this.greigeRollAverager.Add(this.srcData.PatternRepeatLength, this.srcData.FeetCounter);
            this.mapDatumAverager.Add(this.srcData.PatternRepeatLength, this.srcData.FeetCounter);
          }

          break;

        case nameof(this.srcData.PatternRepeatLength):
          if (this.IsMovementForward)
          {
            this.Elongation = this.srcData.PatternRepeatLength; ;
          }

          break;

        case nameof(this.srcData.IsDoffDetected):
          this.DoffDetectedAsync().NoWait();
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

    private async Task KeepBowAndSkewUpToDateAsync()
    {
      for (; ; )
      {
        await Task.Delay(1000);
        if (this.isFeetCounterChanged)
        {
          this.isFeetCounterChanged = false;
          if (this.CurrentCutRoll != null)
          {
            var greigeRoll = this.sewinQueue.Rolls.FirstOrDefault(gr => gr.Id == this.CurrentCutRoll.GreigeRollId);
            if (greigeRoll != null)
            {
              var basOffset = greigeRoll.BasFeetCounterStart - greigeRoll.PrsFeetCounterStart;
              (this.CurrentCutRoll.Bow, this.CurrentCutRoll.Skew) =
                this.dbLocal.GetAverageBowAndSkew(this.CurrentCutRoll.FeetCounterStart + basOffset, this.CurrentCutRoll.FeetCounterEnd + basOffset);
            }

            this.CurrentCutRoll.Elongation = 
              this.dbLocal.GetAverageElongation(this.CurrentCutRoll.FeetCounterStart, this.CurrentCutRoll.FeetCounterEnd);

            this.UpdateInspectionArea();
          }
        }
      }
    }

    internal void UpdateInspectionArea()
    {
      var rollPosition = this.CurrentFeetCounter - this.CurrentRoll.PrsFeetCounterStart;
      var basFirstFoot = (long)(this.CurrentRoll.BasFeetCounterStart + rollPosition + this.serviceSettings.SeamToCutKnife);
      var basLastFoot = basFirstFoot + 50;
      var list = this.dbLocal.GetBowAndSkewMap((long)basFirstFoot, (long)basLastFoot).ToList();
      int j = 0;
      while (j < list.Count || j < this.inspectionAreaList.Count)
      {
        if (j < list.Count)
        {
          var src = list[j];
          InspectionAreaDatum dst;
          if (j < this.inspectionAreaList.Count)
          {
            dst = this.inspectionAreaList[j];
          }
          else
          {
            dst = new InspectionAreaDatum();
            this.inspectionAreaList.Add(dst);
          }

          dst.FeetCounter = list[j].FeetCounter;
          dst.RollPosition = (int)(src.FeetCounter - this.CurrentRoll.BasFeetCounterStart);
          dst.FeetToSeamDetector = (int)(src.FeetCounter - basFirstFoot);
          dst.Bow = list[j].Bow;
          dst.Skew = list[j].Skew;
          j++;
        }
        else
        {
          if (j < this.inspectionAreaList.Count)
          {
            this.inspectionAreaList.RemoveAt(j);
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
      return (index == 0 ? "" : epe < 1.0 ? "+" : "-") + index.ToString(CultureInfo.InvariantCulture);
    }

    private void SaveState(IProgramState state)
    {
      var cutRoll = this.CurrentCutRoll;
      if (cutRoll != null)
      {
        state.Set(nameof(CutRoll), new
        {
          cutRoll.Id,
          cutRoll.GreigeRollId,
          cutRoll.SapRoll,
          cutRoll.FeetCounterStart,
          cutRoll.FeetCounterEnd,
          cutRoll.Bow,
          cutRoll.Skew,
          cutRoll.EPE,
          cutRoll.Dlot,
          cutRoll.Elongation,
        });
      }

      if (this.CurrentCutRoll != null)
      {
        state.Set(typeof(PatternRepeatModel).Name, new
        {
          CurrentCutRollId = this.CurrentCutRoll.Id,
        });
      }
    }

    private async Task DoffDetectedAsync()
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

      if (this.CurrentCutRoll == null || this.CurrentCutRoll.Length >= this.serviceSettings.MinSeamSpacing)
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
      await this.srcData.AcknowledgeDoffDetectAsync(this.ctsDoff.Token);
    }
  }
}
