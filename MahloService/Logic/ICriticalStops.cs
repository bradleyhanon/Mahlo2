using System;

namespace MahloService.Logic
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