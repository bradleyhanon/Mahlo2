using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Opc
{
  interface IPatternRepeatSrc : IMeterSrc<PatternRepeatRoll>
  {
    double PatternRepeatLength { get; }
  }
}
