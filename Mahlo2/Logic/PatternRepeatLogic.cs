using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;

namespace Mahlo.Logic
{
  class PatternRepeatLogic : MeterLogic<PatternRepeatRoll>, IPatternRepeatLogic
  {
    //public PatternRepeatLogic(IPatternRepeatSrc dataSrc, IMeterLogic<PatternRepeatRoll> meterLogic) 
    public PatternRepeatLogic(IPatternRepeatSrc dataSrc, ISewinQueue sewinQueue, IAppInfoBAS appInfo, IUserAttentions<PatternRepeatRoll> userAttentions, ICriticalStops<PatternRepeatRoll> criticalStops, IProgramState programState)
      : base(dataSrc, sewinQueue, appInfo, userAttentions, criticalStops, programState)
    {
      dataSrc.PatternRepeatChanged.Subscribe(value => this.CurrentRoll.Elongation = value);
    }

    public override int Feet
    {
      get => this.CurrentRoll.PrsFeet;
      set => this.CurrentRoll.PrsFeet = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.PrsSpeed;
      set => this.CurrentRoll.PrsSpeed = value;
    }
  }
}
