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
  class MahloLogic : MeterLogic<MahloRoll>, IMahloLogic
  {
    public MahloLogic(ISewinQueue sewinQueue, IMahloSrc srcData, IDbMfg dbMfg, IDbLocal dbLocal, IAppInfoBAS appInfo)
      : base(srcData, sewinQueue, dbMfg, dbLocal, appInfo)
    {
      srcData.Initialize<IMeterSrc>();
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
