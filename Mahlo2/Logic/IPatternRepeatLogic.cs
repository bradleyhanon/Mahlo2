using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IPatternRepeatLogic : IModelLogic
  {
    PatternRepeatRoll CurrentRoll { get; }
    GreigeRoll CurrentGreigeRoll { get; }
  }
}