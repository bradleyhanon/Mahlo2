using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IBowAndSkewLogic : IModelLogic
  {
    BowAndSkewRoll CurrentRoll { get; }
    GreigeRoll CurrentGreigeRoll { get; }
  }
}