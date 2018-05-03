﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;

namespace MapperClient.Logic
{
  class BowAndSkewLogic : ModelLogic, IBowAndSkewLogic
  {
    public BowAndSkewLogic(ISewinQueue sewinQueue)
      : base(sewinQueue)
    {
    }
  }
}
