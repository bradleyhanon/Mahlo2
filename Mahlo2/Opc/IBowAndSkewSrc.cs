using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Opc
{
  interface IBowAndSkewSrc : IMeterSrc<BowAndSkewRoll>
  {
    string Recipe { get; set; }

    IObservable<double> BowChanged { get; }
    IObservable<double> SkewChanged { get; }
    IObservable<bool> OnOffChanged { get; }
  }
}
