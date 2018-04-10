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
  class MahloLogic : MeterLogic, IMahloLogic
  {
    public MahloLogic(IDbMfg dbMfg, IDbLocal dbLocal, ISewinQueue sewinQueue, IMeterSrc srcData, IAppInfoBAS appInfo)
      : base(srcData, sewinQueue, dbMfg, dbLocal, appInfo)
    {
      srcData.Initialize<IMeterSrc>();
    }
  }
}
