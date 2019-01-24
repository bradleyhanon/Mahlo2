using System.Threading;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Opc
{
  internal interface IPatternRepeatSrc : IMeterSrc<PatternRepeatModel>
  {
    bool IsDoffDetected { get; }
    double PatternRepeatLength { get; }

    Task AcknowledgeDoffDetectAsync(CancellationToken token);
  }
}
