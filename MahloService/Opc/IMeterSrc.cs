using System.Threading;
using System.Threading.Tasks;

namespace MahloService.Opc
{
  internal interface IMeterSrc
  {
    double FeetCounter { get; set; }
    double FeetPerMinute { get; }
    double MeasuredWidth { get; }
    bool IsSeamDetected { get; }
    string Recipe { get; }
    bool IsAutoMode { get; }

    Task AcknowledgeSeamDetectAsync(CancellationToken token);
    void SetStatusIndicator(bool value);
    void SetCriticalAlarmIndicator(bool value);
    void SetMiscellaneousIndicator(bool value);
    void SetRecipe(string recipeName);
    void SetAutoMode(bool value);
    Task SetRecipeFromPatternLength(double targetPatternRepeatLength);
  }

  internal interface IMeterSrc<Model> : IMeterSrc
  {
  }
}
