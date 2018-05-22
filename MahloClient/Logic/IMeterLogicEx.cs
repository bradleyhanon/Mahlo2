using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;

namespace MahloClient.Logic
{
  interface IMeterLogicEx : IMeterLogic
  {
    CarpetRoll NextRoll { get; set; }
    CarpetRollTypeEnum CurrentRollType { get; set; }
    CarpetRollTypeEnum? NextRollType { get; set; }
  }
}
