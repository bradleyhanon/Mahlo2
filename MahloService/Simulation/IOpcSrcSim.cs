using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Opc;

namespace MahloService.Simulation
{
  interface IOpcSrcSim<Model> : IMahloSrc, IBowAndSkewSrc, IPatternRepeatSrc
  {
    void Start(double startFootage, double feetPerMinute);
    void Stop();
  }
}
