using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Opc
{
  interface IBowAndSkewSrc : IMeterSrc<BowAndSkewModel>
  {
    double Bow { get; }
    double Skew { get; }
  }
}
