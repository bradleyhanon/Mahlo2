using System;
using System.ComponentModel;

namespace MahloService.Logic
{
  interface ICriticalStops : INotifyPropertyChanged
  {
    bool Any { get; }
    bool IsMahloCommError { get; set; }
    bool IsPlcCommError { get; set; }
  }

  interface ICriticalStops<Model> : ICriticalStops
  {
  }
}