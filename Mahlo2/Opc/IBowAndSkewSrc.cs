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

    bool OnOff { get; set; }
    int Status { get; set; }
    double MeterStamp { get; set; }
    bool BowValid { get; set; }
    double BowInPercent { get; set; }
    bool SkewValid { get; set; }
    double SkewInPercent { get; set; }
    int ControllerState { get; set; }
  }
}
