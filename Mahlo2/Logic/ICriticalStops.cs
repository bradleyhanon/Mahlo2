using System;
using Mahlo.Opc;

namespace Mahlo.Logic
{
  interface ICriticalStops<Model>
  {
    bool Any { get; }
    IObservable<ICriticalStops<Model>> Changes { get; }
    bool IsMahloCommError { get; set; }
    bool IsPlcCommError { get; set; }
    IMeterSrc<Model> MeterSrc { get; set; }
  }
}