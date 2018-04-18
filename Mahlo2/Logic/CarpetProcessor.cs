using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

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
      this.SewinQueue = sewinQueue;
      this.MahloLogic = mahloLogic;
      this.BowAndSkewLogic = bowAndSkewLogic;
      this.PatternRepeatLogic = patternRepeatLogic;
      this.CutRollLogic = cutRollLogic;
    }

    public ISewinQueue SewinQueue { get; }
    public IMahloLogic MahloLogic { get; }
    public IBowAndSkewLogic BowAndSkewLogic { get; }
    public IPatternRepeatLogic PatternRepeatLogic { get; }
    public ICutRollLogic CutRollLogic { get; }

    public int startRollId;

    public void Start()
    {
      //this.mahloLogic.Start();
      //this.bowAndSkewLogic.Start();
      //this.patternRepeatLogic.Start();
    }
  }
}
