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
    IDbLocal dbLocal;
    public PatternRepeatLogic(IDbMfg dbMfg, IDbLocal dbLocal, ISewinQueue sewinQueue, IPatternRepeatSrc srcData, IAppInfoBAS appInfo) 
      : base(srcData, sewinQueue, dbMfg, dbLocal, appInfo)
    {
      this.dbLocal = dbLocal;
      srcData.Initialize<IPatternRepeatSrc>();
    }


    protected override void SaveRoll()
    {
      throw new NotImplementedException();
    }

    protected override void SaveRollMap()
    {
      throw new NotImplementedException();
    }
  }
}
