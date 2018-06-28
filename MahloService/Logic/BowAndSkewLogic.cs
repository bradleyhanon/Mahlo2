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
    private IBowAndSkewSrc dataSrc;

    private double maxBow;
    private double maxSkew;

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
    }

    public override int FeetCounterStart
    {
      get => this.CurrentRoll.BasFeetCounterStart;
      set => this.CurrentRoll.BasFeetCounterStart = value;
    }

    public override int FeetCounterEnd
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

    protected override void OnRollFinished(GreigeRoll greigeRoll)
    {
      base.OnRollFinished(greigeRoll);
      greigeRoll.Bow = this.maxBow;
      greigeRoll.Skew = this.maxSkew;
    }

    protected override void OnRollStarted(GreigeRoll greigeRoll)
    {
      base.OnRollStarted(greigeRoll);
      this.maxBow = 0;
      this.maxSkew = 0;
    }

    protected override void OpcValueChanged(string propertyName)
    {
      switch(propertyName)
      {
        case nameof(this.dataSrc.Bow):
          this.CurrentRoll.Bow = this.dataSrc.Bow;
          if (Math.Abs(this.dataSrc.Bow) > Math.Abs(this.maxBow))
          {
            this.maxBow = this.dataSrc.Bow;
          }

          break;

        case nameof(this.dataSrc.Skew):
          this.CurrentRoll.Skew = this.dataSrc.Skew;
          if (Math.Abs(this.dataSrc.Skew) > Math.Abs(this.maxSkew))
          {
            this.maxSkew = this.dataSrc.Skew;
          }

          break;

        default:
          base.OpcValueChanged(propertyName);
          break;
      }
    }
  }
}
