using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;

namespace MapperClient.Logic
{
  interface IMeterLogicEx : IMeterLogic
  {
    CarpetRoll NextRoll { get; set; }
    CarpetRollTypeEnum CurrentRollType { get; set; }
    CarpetRollTypeEnum? NextRollType { get; set; }
  }
}
