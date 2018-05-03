using System;

namespace Mahlo.Logic
{
  interface ICriticalStops
  {
    bool Any { get; }
    bool IsMahloCommError { get; set; }
    bool IsPlcCommError { get; set; }
  }

  interface ICriticalStops<Model> : ICriticalStops
  {
  }
}