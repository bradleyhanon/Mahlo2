using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Models
{
  class PatternRepeatRoll : MahloRoll
  {
    public double Elongation { get; set; }

    public override MahloRoll Create()
    {
      return new PatternRepeatRoll();
    }
  }
}
