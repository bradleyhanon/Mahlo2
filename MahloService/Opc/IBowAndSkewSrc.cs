using MahloService.Models;

namespace MahloService.Opc
{
  internal interface IBowAndSkewSrc : IMeterSrc<BowAndSkewModel>
  {
    double BowInInches { get; }
    double SkewInInches { get; }
  }
}
