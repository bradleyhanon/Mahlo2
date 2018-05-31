﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Models
{
  interface IPatternRepeatRoll
  {
    int Id { get; }
    string RollNo { get; }
    string StyleCode { get; }
    string StyleName { get; }
    string ColorCode { get; }
    string ColorName { get; }
    string BackingCode { get; }
    int RollLength { get; }
    double RollWidth { get; }
    string DefaultRecipe { get; }
    double PatternRepeatLength { get; }
    string ProductImageURL { get; }

    int Feet { get; set; }
    int Speed { get; set; }
    double Elongation { get; set; }

    bool IsCheckRoll { get; }
  }
}