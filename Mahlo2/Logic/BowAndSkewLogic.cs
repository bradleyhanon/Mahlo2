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
using Mahlo.Utilities;
using Newtonsoft.Json;

namespace Mahlo.Logic
{
  [JsonObject]
  class BowAndSkewLogic : MeterLogic<BowAndSkewRoll>, IBowAndSkewLogic
  {
    public BowAndSkewLogic(
      IBowAndSkewSrc<BowAndSkewRoll> dataSrc, 
      ISewinQueue sewinQueue, 
      IAppInfoBAS appInfo, 
      IUserAttentions<BowAndSkewRoll> userAttentions, 
      ICriticalStops<BowAndSkewRoll> criticalStops, 
      IProgramState programState, 
      ISchedulerProvider schedulerProvider)
      : base(dataSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
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
