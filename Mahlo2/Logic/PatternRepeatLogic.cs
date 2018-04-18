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
  class PatternRepeatLogic : IPatternRepeatLogic
  {
    IMeterLogic<PatternRepeatRoll> meterLogic;
    private IPatternRepeatSrc dataSrc;

    public PatternRepeatLogic(IPatternRepeatSrc dataSrc, IMeterLogic<PatternRepeatRoll> meterLogic) 
    {
      this.dataSrc = dataSrc;
      this.meterLogic = meterLogic;
      this.dataSrc.PatternRepeatChanged.Subscribe(value => this.CurrentRoll.Elongation = value);
    }

    public PatternRepeatRoll CurrentRoll => this.meterLogic.CurrentRoll;
    public GreigeRoll CurrentGreigeRoll => this.meterLogic.CurrentGreigeRoll;
  }
}
