﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Opc
{
  interface IMeterSrc<Model> : INotifyPropertyChanged
  {
    double MetersCount { get; set; }
    double MetersOffset { get; set; }
    double Speed { get; set; }
    bool SeamDetected { get; set; }

    //IObservable<double> MeterCount { get; }
    //IObservable<double> MeterOffset { get; }
    //IObservable<double> Speed { get; }
    //IObservable<bool> SeamDetected { get; }

    void Initialize();

    IObservable<double> MeterCountObservable { get; }
    IObservable<bool> SeamDetectedObservable { get; }
    IObservable<double> SpeedObservable { get; }

    void ResetMeterOffset();
    void ResetSeamDetector();
    void SetStatusIndicator(bool value);
    void SetCriticalAlarm(bool value);
  }
}
