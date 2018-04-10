using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IMeterLogic
  {
    GreigeRoll CurrentGreigeRoll { get; }
    MahloRoll CurrentRoll { get; }
    List<MahloRoll> RollMap { get; }
    List<MahloRoll> Rolls { get; }
  }
}
