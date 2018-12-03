using MahloClient.Ipc;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;

namespace MahloClient.Logic
{
  internal class MahloLogic : MeterLogic<MahloModel>, IMahloLogic
  {
    public MahloLogic(IMahloIpcClient ipcClient, ISewinQueue sewinQueue, IServiceSettings serviceSettings)
      : base(ipcClient, sewinQueue, serviceSettings)
    {
    }

    public override string InterfaceName => nameof(IMahloLogic);

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
  }
}
