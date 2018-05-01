using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;

namespace MapperClient.Logic
{
  interface ICarpetProcessor
  {
    IBowAndSkewLogic BowAndSkewLogic { get; }
    ICutRollLogic CutRollLogic { get; }
    IMahloLogic MahloLogic { get; }
    IPatternRepeatLogic PatternRepeatLogic { get; }
    ISewinQueue SewinQueue { get; }

    void Start();
  }
}
