using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;

namespace Mahlo.Logic
{
  class BowAndSkewLogic : MeterLogic<BowAndSkewRoll>, IBowAndSkewLogic
  {
    public BowAndSkewLogic(IDbMfg dbMfg, IDbLocal dbLocal, ISewinQueue sewinQueue, IBowAndSkewSrc srcData, IAppInfoBAS appInfo) 
      : base(srcData, sewinQueue, dbMfg, dbLocal, appInfo)
    {
      srcData.Initialize<IBowAndSkewSrc>();
    }

    protected override void LoadRolls()
    {
      throw new NotImplementedException();
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
