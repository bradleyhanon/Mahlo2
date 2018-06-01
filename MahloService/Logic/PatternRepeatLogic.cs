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
    IPatternRepeatSrc dataSrc;

    //public PatternRepeatLogic(IPatternRepeatSrc dataSrc, IMeterLogic<PatternRepeatRoll> meterLogic) 
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

    protected override void OpcValueChanged(string propertyName)
    {
      switch(propertyName)
      {
        case nameof(this.dataSrc.PatternRepeatLength):
          this.CurrentRoll.PatternRepeatLength = this.dataSrc.PatternRepeatLength;
          break;

        default:
          base.OpcValueChanged(propertyName);
          break;
      }
    }
  }
}
