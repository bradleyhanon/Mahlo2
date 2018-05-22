using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;

namespace MahloClient.Logic
{
  class CarpetProcessor : ICarpetProcessor
  {
    public IBowAndSkewLogic BowAndSkewLogic { get; private set; }

    public ICutRollLogic CutRollLogic { get; private set; }

    public IMahloLogic MahloLogic { get; private set; }

    public IPatternRepeatLogic PatternRepeatLogic { get; private set; }

    public ISewinQueue SewinQueue { get; private set; }

    public CarpetProcessor(
      ISewinQueue sewinQueue,
      IMahloLogic mahloLogic,
      IBowAndSkewLogic bowAndSkewLogic, 
      IPatternRepeatLogic patternRepeatLogic)
    {
      this.SewinQueue = sewinQueue;
      this.MahloLogic = mahloLogic;
      this.BowAndSkewLogic = bowAndSkewLogic;
      this.PatternRepeatLogic = patternRepeatLogic;
    }

    public void Start()
    {
      
    }
  }
}
