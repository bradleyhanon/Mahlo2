using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Models
{
  class CutRoll
  {
    public int RollId { get; set; }
    public string GreigeRoll { get; set; }
    public string SapRoll { get; set; }
    public int Length { get; set; }
    public float MaxBow { get; set; }
    public float MaxSkew { get; set; }
    public float MaxEPE { get; set; }
    public string Dlot { get; set; }
  }
}
