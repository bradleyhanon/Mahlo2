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

    public PatternRepeatLogic(IMeterLogic<PatternRepeatRoll> meterLogic) 
    {
      this.meterLogic = meterLogic;
      //srcData.Initialize<IPatternRepeatSrc>();
    }
  }
}
