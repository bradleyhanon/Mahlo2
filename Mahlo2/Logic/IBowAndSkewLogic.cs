using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IBowAndSkewLogic : IModelLogic
  {
    CarpetRoll CurrentRoll { get; }
  }
}