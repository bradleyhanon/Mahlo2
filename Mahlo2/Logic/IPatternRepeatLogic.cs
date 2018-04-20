using Mahlo.Models;

namespace Mahlo.Logic
{
  interface IPatternRepeatLogic : IModelLogic
  {
    CarpetRoll CurrentRoll { get; }
  }
}