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
  class PatternRepeatLogic : MeterLogic<PatternRepeatRoll>, IPatternRepeatLogic
  {
    public PatternRepeatLogic(IMahloIpcClient ipcClient, ISewinQueue sewinQueue, IServiceSettings serviceSettings)
      : base(ipcClient, sewinQueue, serviceSettings)
    {
    }

    public override string InterfaceName => nameof(IPatternRepeatLogic);

    //public override int Feet
    //{
    //  get => this.CurrentRoll.PrsFeet;
    //  set => throw new NotImplementedException();
    //}

    //public override int Speed
    //{
    //  get => this.CurrentRoll.PrsSpeed;
    //  set => throw new NotImplementedException();
    //}

    //public override bool IsMapValid
    //{
    //  get => this.CurrentRoll.PrsMapValid;
    //  set => throw new NotImplementedException();
    //}
  }
}
