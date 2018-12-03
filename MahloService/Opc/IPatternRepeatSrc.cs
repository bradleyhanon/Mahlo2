using MahloService.Models;

namespace MahloService.Opc
{
  internal interface IPatternRepeatSrc : IMeterSrc<PatternRepeatModel>
  {
    bool IsDoffDetected { get; }
    double PatternRepeatLength { get; }

    void AcknowledgeDoffDetect();
  }
}
