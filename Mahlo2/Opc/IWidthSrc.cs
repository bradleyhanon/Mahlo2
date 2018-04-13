using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Opc
{
  interface IWidthSrc<Model> : IMeterSrc<Model>
  {
    bool OnOff { get; set; }
    int Status { get; set; }
    double MeterStamp { get; set; }
    bool Valid { get; set; }
    double ValueInMeter { get; set; }
  }
}
