using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace MahloService.Models
{
  [Table("PatternRepeatMap")]
  class PatternRepeatMapDatum
  {
    [ExplicitKey]
    public long FeetCounter { get; set; }
    public double Elongation { get; set; }
  }
}
