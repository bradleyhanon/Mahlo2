using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Opc
{
  interface IMeterSrc : INotifyPropertyChanged
  {
    double FeetCounter { get; }
    double FeetPerMinute { get; }
    double MeasuredWidth { get; }
    bool IsSeamDetected { get; }
    string Recipe { get; }
    bool IsAutoMode { get; }

    void AcknowledgeSeamDetect();
    void SetStatusIndicator(bool value);
    void SetCriticalAlarmIndicator(bool value);
    void SetMiscellaneousIndicator(bool value);
    void SetRecipe(string recipeName);
    void SetAutoMode(bool value);
  }

  interface IMeterSrc<Model> : IMeterSrc
  {
  }
}
