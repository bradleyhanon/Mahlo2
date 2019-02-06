using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace MahloService.Models
{
  [AddINotifyPropertyChangedInterface]
  class InspectionAreaDatum
  {
    public long FeetCounter { get; set; }
    public int RollPosition { get; set; }
    public int FeetToSeamDetector { get; set; }
    public double Bow { get; set; }
    public double Skew { get; set; }
  }
}
