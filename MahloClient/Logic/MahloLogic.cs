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
  class MahloLogic : MeterLogic<MahloRoll>, IMahloLogic
  {
    public MahloLogic(IMahloIpcClient ipcClient, ISewinQueue sewinQueue, IServiceSettings serviceSettings)
      : base(ipcClient, sewinQueue, serviceSettings)
    {
    }

    public override string InterfaceName => nameof(IMahloLogic);

    public override int MeasuredLength
    {
      get => this.CurrentRoll.MalFeet;
      set => this.CurrentRoll.MalFeet = value;
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
