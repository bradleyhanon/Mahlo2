﻿namespace MahloService.Opc
{
  internal interface IMeterSrc
  {
    double FeetCounter { get; set; }
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

  internal interface IMeterSrc<Model> : IMeterSrc
  {
  }
}
