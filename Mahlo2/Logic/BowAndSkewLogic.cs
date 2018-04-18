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
    IBowAndSkewSrc bowAndSkewSrc;
    IMeterLogic<BowAndSkewRoll> meterLogic;

    public BowAndSkewLogic(IBowAndSkewSrc bowAndSkewSrc, IMeterLogic<BowAndSkewRoll> meterLogic) 
    {
      this.bowAndSkewSrc = bowAndSkewSrc;
      this.meterLogic = meterLogic;
      this.bowAndSkewSrc.BowChanged.Subscribe(value => this.CurrentRoll.Bow = value);
      this.bowAndSkewSrc.SkewChanged.Subscribe(value => this.CurrentRoll.Skew = value);
    }

    public BowAndSkewRoll CurrentRoll => this.meterLogic.CurrentRoll;
    public GreigeRoll CurrentGreigeRoll => this.meterLogic.CurrentGreigeRoll;

  }
}
