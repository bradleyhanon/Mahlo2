using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Settings;

namespace MahloService.Logic
{
  internal class BowAndSkewLogic : MeterLogic<BowAndSkewModel>, IBowAndSkewLogic
  {
    private readonly IDbLocal dbLocal;
    private readonly IBowAndSkewSrc dataSrc;
    private readonly BowAndSkewMapDatum mapDatum = new BowAndSkewMapDatum();

    private readonly Averager bowAverager = new Averager();
    private readonly Averager skewAverager = new Averager();
    private readonly Averager bowMapAverager = new Averager();
    private readonly Averager skewMapAverager = new Averager();

    public BowAndSkewLogic(
      IDbLocal dbLocal,
      IBowAndSkewSrc dataSrc,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<BowAndSkewModel> userAttentions,
      ICriticalStops<BowAndSkewModel> criticalStops,
      IProgramState programState,
      IScheduler scheduler)
      : base(dbLocal, dataSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, scheduler)
    {
      this.dbLocal = dbLocal;
      this.dataSrc = dataSrc;
      this.SeamDelayLine.DelayTicks = appInfo.SeamToBowAndSkew;
    }

    public override long FeetCounterStart
    {
      get => this.CurrentRoll.BasFeetCounterStart;
      set => this.CurrentRoll.BasFeetCounterStart = value;
    }

    public override long FeetCounterEnd
    {
      get => this.CurrentRoll.BasFeetCounterEnd;
      set => this.CurrentRoll.BasFeetCounterEnd = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.BasSpeed;
      set => this.CurrentRoll.BasSpeed = value;
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.BasMapValid;
      set => this.CurrentRoll.BasMapValid = value;
    }

    public double Bow
    {
      get => this.CurrentRoll.Bow;
      set => this.CurrentRoll.Bow = value;
    }

    public double Skew
    {
      get => this.CurrentRoll.Skew;
      set => this.CurrentRoll.Skew = value;
    }

    protected override string MapTableName => "BowAndSkewMap";

    public override Task ApplyRecipeAsync(string recipeName, bool isManualMode)
    {
      if (isManualMode)
      {
        if (this.IsManualMode)
        {
          this.dataSrc.SetAutoMode(false);
        }
        else
        {
          this.dataSrc.SetRecipe(recipeName);
          this.dataSrc.SetAutoMode(true);
        }
      }

      return Task.CompletedTask;
    }

    protected override GreigeRoll FindCurrentRollOnStartup(ISewinQueue sewinQueue)
    {
      return sewinQueue.Rolls.LastOrDefault(roll => roll.BasFeetCounterEnd != 0);
    }

    protected override void SaveMapDatum()
    {
      if (this.bowMapAverager.Count > 0)
      {
        this.mapDatum.FeetCounter = this.CurrentFeetCounter;
        this.mapDatum.Bow = this.bowMapAverager.Average;
        this.mapDatum.Skew = this.skewMapAverager.Average;
        this.dbLocal.InsertBowAndSkewMapDatum(this.mapDatum);
      }

      this.bowMapAverager.Clear();
      this.skewMapAverager.Clear();
    }

    protected override void OnRollFinished(GreigeRoll greigeRoll)
    {
      base.OnRollFinished(greigeRoll);
      greigeRoll.Bow = this.bowAverager.Average;
      greigeRoll.Skew = this.skewAverager.Average;
    }

    protected override void OnRollStarted(GreigeRoll greigeRoll)
    {
      base.OnRollStarted(greigeRoll);
      this.bowAverager.Clear();
      this.skewAverager.Clear();
    }

    protected override void OpcValueChanged(string propertyName)
    {
      base.OpcValueChanged(propertyName);

      switch (propertyName)
      {
        case nameof(this.dataSrc.FeetCounter):
          if (this.IsMovementForward)
          {
            this.bowAverager.Add(Math.Abs(this.dataSrc.BowInInches), this.dataSrc.FeetCounter);
            this.skewAverager.Add(Math.Abs(this.dataSrc.SkewInInches), this.dataSrc.FeetCounter);
            this.bowMapAverager.Add(Math.Abs(this.dataSrc.BowInInches), this.dataSrc.FeetCounter);
            this.skewMapAverager.Add(Math.Abs(this.dataSrc.SkewInInches), this.dataSrc.FeetCounter);
          }

          break;

        case nameof(this.dataSrc.BowInInches):
          this.CurrentRoll.Bow = this.dataSrc.BowInInches;
          break;

        case nameof(this.dataSrc.SkewInInches):
          this.CurrentRoll.Skew = this.dataSrc.SkewInInches;
          break;
      }
    }
  }
}
