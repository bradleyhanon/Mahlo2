using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Utilities;
using Newtonsoft.Json;

namespace MahloService.Logic
{
  class BowAndSkewLogic : MeterLogic<BowAndSkewRoll>, IBowAndSkewLogic
  {
    public BowAndSkewLogic(
      IBowAndSkewSrc<BowAndSkewRoll> dataSrc, 
      ISewinQueue sewinQueue, 
      IServiceSettings appInfo, 
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

    public override bool IsMapValid
    {
      get => this.CurrentRoll.BasMapValid;
      set => this.CurrentRoll.BasMapValid = value;
    }
  }
}
