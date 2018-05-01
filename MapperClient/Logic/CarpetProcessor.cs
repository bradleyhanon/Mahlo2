using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;

namespace MapperClient.Logic
{
  class CarpetProcessor : ICarpetProcessor
  {
    IBowAndSkewLogic ICarpetProcessor.BowAndSkewLogic => throw new NotImplementedException();

    ICutRollLogic ICarpetProcessor.CutRollLogic => throw new NotImplementedException();

    IMahloLogic ICarpetProcessor.MahloLogic => throw new NotImplementedException();

    IPatternRepeatLogic ICarpetProcessor.PatternRepeatLogic => throw new NotImplementedException();

    ISewinQueue ICarpetProcessor.SewinQueue => throw new NotImplementedException();

    void ICarpetProcessor.Start()
    {
      throw new NotImplementedException();
    }
  }
}
