using MahloService.Models;

namespace MahloService.Opc
{
  internal interface IBowAndSkewSrc : IMeterSrc<BowAndSkewModel>
  {
    double Bow { get; }
    double Skew { get; }
  }
}
