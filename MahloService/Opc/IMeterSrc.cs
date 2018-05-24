using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Opc
{
  interface IMeterSrc<Model>
  {
    string Recipe { get; set; }
    bool IsAutoMode { get; set; }

    IObservable<int> FeetCounter { get; }
    IObservable<int> FeetPerMinute { get; }
    IObservable<double> WidthChanged { get; }
    IObservable<bool> SeamDetected { get; }

    void ResetMeterOffset();
    void ResetSeamDetector();
    void SetStatusIndicator(bool value);
    void SetCriticalAlarm(bool value);
    void SetRecipe(string recipeName);
    void SetAutoMode(bool value);
  }
}
