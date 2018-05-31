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
    IBowAndSkewSrc dataSrc;
    public BowAndSkewLogic(
      IBowAndSkewSrc dataSrc,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<BowAndSkewRoll> userAttentions,
      ICriticalStops<BowAndSkewRoll> criticalStops,
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
      : base(dataSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
    {
      this.dataSrc = dataSrc;
      dataSrc.BowChanged.Subscribe(value => this.CurrentRoll.Bow = value * this.MeasuredWidth);
      dataSrc.SkewChanged.Subscribe(value => this.CurrentRoll.Skew = value * this.MeasuredWidth);
    }

    public override int MeasuredLength
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

    public override Task ApplyRecipe(string recipeName, bool isManualMode)
    {
      if (isManualMode)
      {
        if (this.IsManualMode)
        {
          this.dataSrc.SetAutoMode(false);
        }
        else
        {
          this.dataSrc.SetRecipe(recipeName);
          this.dataSrc.SetAutoMode(true);
        }
      }

      return Task.CompletedTask;
    }
  }
}
