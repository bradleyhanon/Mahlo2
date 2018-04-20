using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;

namespace Mahlo.Logic
{
  class BowAndSkewLogic : MeterLogic<BowAndSkewRoll>, IBowAndSkewLogic
  {
    public BowAndSkewLogic(IBowAndSkewSrc dataSrc, ISewinQueue sewinQueue, IAppInfoBAS appInfo, IUserAttentions<BowAndSkewRoll> userAttentions, ICriticalStops<BowAndSkewRoll> criticalStops, IProgramState programState)
      : base(dataSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState)
    {
      dataSrc.BowChanged.Subscribe(value => this.CurrentRoll.Bow = value);
      dataSrc.SkewChanged.Subscribe(value => this.CurrentRoll.Skew = value);
    }

    public override int Feet
    {
      get => this.CurrentRoll.BasFeet;
      set => this.CurrentRoll.BasFeet = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.BasSpeed;
      set => this.CurrentRoll.BasSpeed = value;
    }
  }
}
