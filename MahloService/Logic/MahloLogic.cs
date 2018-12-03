using System.Linq;
using System.Reactive.Concurrency;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Settings;

namespace MahloService.Logic
{
  internal class MahloLogic : MeterLogic<MahloModel>, IMahloLogic
  {
    private readonly IDbLocal dbLocal;
    private readonly Mahlo2MapDatum mapDatum = new Mahlo2MapDatum();

    public MahloLogic(
      IDbLocal dbLocal,
      IMahloSrc mahloSrc,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<MahloModel> userAttentions,
      ICriticalStops<MahloModel> criticalStops,
      IProgramState programState,
      IScheduler scheduler)
      : base(dbLocal, mahloSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, scheduler)
    {
      this.dbLocal = dbLocal;
    }

    public override long FeetCounterStart
    {
      get => this.CurrentRoll.MalFeetCounterStart;
      set => this.CurrentRoll.MalFeetCounterStart = value;
    }

    public override long FeetCounterEnd
    {
      get => this.CurrentRoll.MalFeetCounterEnd;
      set => this.CurrentRoll.MalFeetCounterEnd = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.MalSpeed;
      set => this.CurrentRoll.MalSpeed = value;
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.MalMapValid;
      set => this.CurrentRoll.MalMapValid = value;
    }

    protected override string MapTableName => "Mahlo2Map";

    protected override GreigeRoll FindCurrentRollOnStartup(ISewinQueue sewinQueue)
    {
      return sewinQueue.Rolls.LastOrDefault(roll => roll.MalFeetCounterEnd != 0);
    }

    protected override void SaveMapDatum()
    {
      this.mapDatum.FeetCounter = this.CurrentFeetCounter;
      this.dbLocal.InsertMahlo2MapDatum(this.mapDatum);
    }
  }
}
