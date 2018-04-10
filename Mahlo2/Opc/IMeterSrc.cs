using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Opc
{
  interface IMeterSrc : INotifyPropertyChanged
  {
    double MetersCount { get; set; }
    double MetersOffset { get; set; }
    double Speed { get; set; }
    bool SeamDetected { get; set; }

    //IObservable<double> MeterCount { get; }
    //IObservable<double> MeterOffset { get; }
    //IObservable<double> Speed { get; }
    //IObservable<bool> SeamDetected { get; }

    void Initialize<I>() where I : IMeterSrc;
    void ResetMeterOffset();
    void ResetSeamDetector();
  }
}
