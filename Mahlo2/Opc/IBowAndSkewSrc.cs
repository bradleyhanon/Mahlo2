using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Opc
{
  interface IBowAndSkewSrc<Model> : IMeterSrc<Model>
  {
    IObservable<double> BowChanged { get; }
    IObservable<double> SkewChanged { get; }
  }
}
