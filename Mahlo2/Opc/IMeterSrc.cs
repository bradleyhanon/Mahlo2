using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Opc
{
  interface IMeterSrc<Model>
  {
    string Recipe { get; set; }
    bool IsManualMode { get; set; }

    IObservable<int> FeetCounter { get; }
    IObservable<int> FeetPerMinute { get; }
    IObservable<bool> SeamDetected { get; }

    void ResetMeterOffset();
    void ResetSeamDetector();
    void SetStatusIndicator(bool value);
    void SetCriticalAlarm(bool value);
  }
}
