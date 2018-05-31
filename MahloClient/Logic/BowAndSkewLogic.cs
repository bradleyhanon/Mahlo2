﻿using System;
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
  class BowAndSkewLogic : MeterLogic<BowAndSkewRoll>, IBowAndSkewLogic
  {
    public BowAndSkewLogic(IMahloIpcClient ipcClient, ISewinQueue sewinQueue, IServiceSettings serviceSettings)
      : base(ipcClient, sewinQueue, serviceSettings)
    {
    }

    public override string InterfaceName => nameof(IBowAndSkewLogic);

    //public override int Feet
    //{
    //  get => this.CurrentRoll.BasFeet;
    //  set => throw new NotImplementedException();
    //}

    //public override int Speed
    //{
    //  get => this.CurrentRoll.BasSpeed;
    //  set => throw new NotImplementedException();
    //}

    //public override bool IsMapValid
    //{
    //  get => this.CurrentRoll.BasMapValid;
    //  set => throw new NotImplementedException();
    //}
  }
}