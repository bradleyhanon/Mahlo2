using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;

namespace MahloClient.Logic
{
  class MahloLogic : MeterLogic<MahloRoll>, IMahloLogic
  {
    public MahloLogic(ISewinQueue sewinQueue)
      : base(sewinQueue)
    {
    }

    public override int Feet
    {
      get => this.CurrentRoll.MalFeet;
      set => throw new NotImplementedException();
    }
    public override int Speed
    {
      get => this.CurrentRoll.MalSpeed;
      set => throw new NotImplementedException();
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.MalMapValid;
      set => throw new NotImplementedException();
    }
  }
}
