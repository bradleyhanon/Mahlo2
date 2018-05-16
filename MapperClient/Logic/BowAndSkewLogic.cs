using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;

namespace MapperClient.Logic
{
  class BowAndSkewLogic : MeterLogic<BowAndSkewRoll>, IBowAndSkewLogic
  {
    public BowAndSkewLogic(ISewinQueue sewinQueue)
      : base(sewinQueue)
    {
    }

    public override int Feet
    {
      get => this.CurrentRoll.BasFeet;
      set => throw new NotImplementedException();
    }

    public override int Speed
    {
      get => this.CurrentRoll.BasSpeed;
      set => throw new NotImplementedException();
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.BasMapValid;
      set => throw new NotImplementedException();
    }
  }
}
