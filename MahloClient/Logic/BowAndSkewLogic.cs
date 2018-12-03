using MahloClient.Ipc;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;

namespace MahloClient.Logic
{
  internal class BowAndSkewLogic : MeterLogic<BowAndSkewModel>, IBowAndSkewLogic
  {
    public BowAndSkewLogic(IMahloIpcClient ipcClient, ISewinQueue sewinQueue, IServiceSettings serviceSettings)
      : base(ipcClient, sewinQueue, serviceSettings)
    {
    }

    public override string InterfaceName => nameof(IBowAndSkewLogic);

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
  }
}
