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
  class BowAndSkewLogic : MeterLogic<BowAndSkewModel>, IBowAndSkewLogic
  {
    private readonly IDbLocal dbLocal;
    private readonly IBowAndSkewSrc dataSrc;
    private readonly BowAndSkewMapDatum mapDatum = new BowAndSkewMapDatum();

    private double maxBow;
    private double maxSkew;

    public BowAndSkewLogic(
      IDbLocal dbLocal,
      IBowAndSkewSrc dataSrc,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<BowAndSkewModel> userAttentions,
      ICriticalStops<BowAndSkewModel> criticalStops,
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
      : base(dbLocal, dataSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
    {
      this.dbLocal = dbLocal;
      this.dataSrc = dataSrc;
    }

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

    protected override string MapTableName => "BowAndSkewMap";

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

    protected override void SaveMapDatum()
    {
      this.mapDatum.FeetCounter = this.CurrentFeetCounter;
      this.dbLocal.InsertBowAndSkewMapDatum(this.mapDatum);
      this.mapDatum.Bow = 0.0;
      this.mapDatum.Skew = 0.0;
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
          if (this.IsMovementForward)
          {
            this.CurrentRoll.Bow = this.dataSrc.Bow;
            if (Math.Abs(this.dataSrc.Bow) > Math.Abs(this.maxBow))
            {
              this.maxBow = this.dataSrc.Bow;
            }

            if (Math.Abs(this.dataSrc.Bow) > Math.Abs(this.mapDatum.Bow))
            {
              this.mapDatum.Bow = this.dataSrc.Bow;
            }
          }

          break;

        case nameof(this.dataSrc.Skew):
          if (this.IsMovementForward)
          {
            this.CurrentRoll.Skew = this.dataSrc.Skew;
            if (Math.Abs(this.dataSrc.Skew) > Math.Abs(this.maxSkew))
            {
              this.maxSkew = this.dataSrc.Skew;
            }

            if (Math.Abs(this.dataSrc.Skew) > Math.Abs(this.mapDatum.Skew))
            {
              this.mapDatum.Skew = this.dataSrc.Skew;
            }
          }

          break;

        default:
          base.OpcValueChanged(propertyName);
          break;
      }
    }
  }
}
