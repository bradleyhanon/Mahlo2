using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace MahloService.Models
{
  class CutRoll
  {
    public int RollId { get; set; }
    public int GreigeRollId { get; set; }
    public string SapRoll { get; set; }
    public int FeetCounterStart { get; set; }
    public int FeetCounterEnd { get; set; }

    [DependsOn(nameof(FeetCounterEnd))]
    public int Length => this.FeetCounterEnd - this.FeetCounterStart;

    public double MaxBow { get; set; }
    public double MaxSkew { get; set; }
    public double MaxEPE { get; set; }
    public string Dlot { get; set; }
  }
}
