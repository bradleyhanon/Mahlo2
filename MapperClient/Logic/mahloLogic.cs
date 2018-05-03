﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;

namespace MapperClient.Logic
{
  class MahloLogic : ModelLogic, IMahloLogic
  {
    public MahloLogic(ISewinQueue sewinQueue)
      : base(sewinQueue)
    {
    }
  }
}
