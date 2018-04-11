using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Logic
{
  class CarpetProcessor
  {
    public CarpetProcessor(
      ISewinQueue sewinQueue,
      IMahloLogic mahloLogic,
      IBowAndSkewLogic bowAndSkewLogic,
      IPatternRepeatLogic patternRepeatLogic,
      ICutRollLogic cutRollLogic)
    {
      this.sewinQueue = sewinQueue;
      this.mahloLogic = mahloLogic;
      this.bowAndSkewLogic = bowAndSkewLogic;
      this.patternRepeatLogic = patternRepeatLogic;
      this.cutRollLogic = cutRollLogic;
    }

    public ISewinQueue sewinQueue { get; }
    public IMahloLogic mahloLogic { get; }
    public IBowAndSkewLogic bowAndSkewLogic { get; }
    public IPatternRepeatLogic patternRepeatLogic { get; }
    public ICutRollLogic cutRollLogic { get; }

    public int startRollId;

    public void Start()
    {
      this.mahloLogic.Start();
      this.bowAndSkewLogic.Start();
      this.patternRepeatLogic.Start();
    }
  }
}
