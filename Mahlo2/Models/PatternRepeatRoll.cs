using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Models
{
  class PatternRepeatRoll : MahloRoll
  {
    public int Elongation { get; set; }

    public override MahloRoll Create()
    {
      return new PatternRepeatRoll();
    }
  }
}
