using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;
using Mahlo.Repository;

namespace Mahlo.Controllers
{
  class ProductionLine
  {
    /// <summary>
    /// Distance with which seam detects are ignored
    /// </summary>
    const int SeamDetectThreshold = 5; 

    private ISewinQueue sewinQueue;

    private Mahlo2Roll currentRoll;

    private IDbLocal dbLocal;

    public ProductionLine(IDbLocal dbLocal, ISewinQueue sewinQueue, IMahlo2 machlo2, IBowAndSkew bowAndSkew, IPatternRepeat patternRepeat, )
    {
      this.dbLocal = dbLocal;
      this.sewinQueue = sewinQueue;
    }

    public virtual void DoSeamDetected()
    {

    }

    public virtual void SetMeter(int value)
    {

    }
  }
}
