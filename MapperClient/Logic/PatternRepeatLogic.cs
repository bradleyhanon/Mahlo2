﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;

namespace MapperClient.Logic
{
  class PatternRepeatLogic : MeterLogic<PatternRepeatRoll>, IPatternRepeatLogic
  {
    public PatternRepeatLogic(ISewinQueue sewinQueue)
      : base(sewinQueue)
    {
    }

    public override int Feet
    {
      get => this.CurrentRoll.PrsFeet;
      set => throw new NotImplementedException();
    }

    public override int Speed
    {
      get => this.CurrentRoll.PrsSpeed;
      set => throw new NotImplementedException();
    }
  }
}
