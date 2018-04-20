using System.Collections.Generic;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IMahloLogic : IModelLogic
  {
    CarpetRoll CurrentRoll { get; }
  }
}