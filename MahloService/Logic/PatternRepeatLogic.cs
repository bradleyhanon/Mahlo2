using System;
using System.Collections.Generic;
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
  class PatternRepeatLogic : MeterLogic<PatternRepeatRoll>, IPatternRepeatLogic
  {
    private IPatternRepeatSrc dataSrc;

    private double maxElongation;

    public PatternRepeatLogic(
      IPatternRepeatSrc dataSrc,
      ISewinQueue sewinQueue,
      IServiceSettings appInfo,
      IUserAttentions<PatternRepeatRoll> userAttentions,
      ICriticalStops<PatternRepeatRoll> criticalStops,
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
      : base(dataSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
    {
      this.dataSrc = dataSrc;
    }

    public override int MeasuredLength
    {
      get => this.CurrentRoll.PrsFeet;
      set => this.CurrentRoll.PrsFeet = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.PrsSpeed;
      set => this.CurrentRoll.PrsSpeed = value;
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.PrsMapValid;
      set => this.CurrentRoll.PrsMapValid = value;
    }

    protected override void OnRollFinished(CarpetRoll carpetRoll)
    {
      base.OnRollFinished(carpetRoll);
      carpetRoll.Elongation = this.maxElongation;
    }

    protected override void OnRollStarted(CarpetRoll carpetRoll)
    {
      base.OnRollStarted(carpetRoll);
      this.maxElongation = 0;
    }

    protected override void OpcValueChanged(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(this.dataSrc.PatternRepeatLength):
          this.CurrentRoll.PatternRepeatLength = this.dataSrc.PatternRepeatLength;
          double elongation = dataSrc.PatternRepeatLength - this.CurrentRoll.PatternRepeatLength;
          if (Math.Abs(elongation) > Math.Abs(this.maxElongation))
          {
            this.maxElongation = elongation;
          }

          break;

        default:
          base.OpcValueChanged(propertyName);
          break;
      }
    }
  }
}
