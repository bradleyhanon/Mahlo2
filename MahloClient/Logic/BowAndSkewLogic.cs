using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloClient.Ipc;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;

namespace MahloClient.Logic
{
  class BowAndSkewLogic : MeterLogic<BowAndSkewModel>, IBowAndSkewLogic
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
  }
}
