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
  class BowAndSkewLogic : IBowAndSkewLogic
  {
    IMeterLogic<BowAndSkewRoll> meterLogic;
    public BowAndSkewLogic(IMeterLogic<BowAndSkewRoll> meterLogic) 
    {
      this.meterLogic = meterLogic;
    }
  }
}
