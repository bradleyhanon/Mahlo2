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
  class MahloLogic : IMahloLogic
  {
    IMeterLogic<MahloRoll> meterLogic;

    public MahloLogic(IMeterLogic<MahloRoll> meterLogic)
    {
      //srcData.Initialize<IMeterSrc>();
    }
  }
}
