using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Opc;
using Mahlo.Repository;

namespace Mahlo.Logic
{
  class PatternRepeatLogic : MeterLogic, IPatternRepeatLogic
  {
    public PatternRepeatLogic(IDbMfg dbMfg, IDbLocal dbLocal, ISewinQueue sewinQueue, IPatternRepeatSrc srcData, IAppInfoBAS appInfo) 
      : base(srcData, sewinQueue, dbMfg, dbLocal, appInfo)
    {
      srcData.Initialize<IPatternRepeatSrc>();
    }
  }
}
