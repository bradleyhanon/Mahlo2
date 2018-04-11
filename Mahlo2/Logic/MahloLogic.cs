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
    IDbLocal dbLocal;
    public MahloLogic(ISewinQueue sewinQueue, IMahloSrc srcData, IDbMfg dbMfg, IDbLocal dbLocal, IAppInfoBAS appInfo)
      : base(srcData, sewinQueue, dbMfg, dbLocal, appInfo)
    {
      this.dbLocal = dbLocal;
      srcData.Initialize<IMeterSrc>();
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
