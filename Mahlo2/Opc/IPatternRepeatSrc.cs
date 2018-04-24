﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Opc
{
  interface IPatternRepeatSrc<Model> : IMeterSrc<Model>
  {
    IObservable<double> PatternRepeatChanged { get; }
  }
}
