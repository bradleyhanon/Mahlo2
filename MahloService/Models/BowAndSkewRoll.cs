using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Models
{
  class BowAndSkewRoll : MahloRoll
  {
    public double Bow { get; set; }
    public double Skew { get; set; }

    public override MahloRoll Create()
    {
      return new BowAndSkewRoll();
    }
  }
}
