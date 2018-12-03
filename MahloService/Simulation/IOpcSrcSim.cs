using MahloService.Opc;

namespace MahloService.Simulation
{
  internal interface IOpcSrcSim<Model> : IMahloSrc, IBowAndSkewSrc, IPatternRepeatSrc
  {
    void Start(double startFootage, double feetPerMinute);
    void Stop();
  }
}
