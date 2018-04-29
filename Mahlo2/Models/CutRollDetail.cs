using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Models
{
  class CutRollDetail
  {
    public string GreigeRoll { get; set; }
    public string DateTime { get; set; }
    public int Position { get; set; }
    public float MaxBow { get; set; }
    public float MaxSkew { get; set; }
    public float Elongation { get; set; }
  }
}
