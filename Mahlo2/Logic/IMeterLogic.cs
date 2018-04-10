using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IMeterLogic<Model>
  {
    GreigeRoll CurrentGreigeRoll { get; }
    Model CurrentRoll { get; }
    List<Model> RollMap { get; }
    List<Model> Rolls { get; }
  }
}
