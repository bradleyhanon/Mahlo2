﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Opc
{
  interface IPatternRepeatSrc : IMeterSrc<PatternRepeatRoll>
  {
    bool OnOff { get; set; }
    int Status { get; set; }
    double MeterStamp { get; set; }

    bool Valid { get; set; }
    double ValueInMeter { get; set; }
    int ControllerState { get; set; }
  }
}
