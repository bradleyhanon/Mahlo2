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
    IMahloSrc mahloSrc;
    IMeterLogic<MahloRoll> meterLogic;

    public MahloLogic(IMahloSrc mahloSrc, IMeterLogic<MahloRoll> meterLogic)
    {
      this.mahloSrc = mahloSrc;
      this.meterLogic = meterLogic;
    }

    public MahloRoll CurrentRoll => this.meterLogic.CurrentRoll;
    public GreigeRoll CurrentGreigeRoll => this.meterLogic.CurrentGreigeRoll;
  }
}
