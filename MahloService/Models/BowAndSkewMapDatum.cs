using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace MahloService.Models
{
  [Table("BowAndSkewMap")]
  class BowAndSkewMapDatum
  {
    [ExplicitKey]
    public long FeetCounter { get; set; }
    public double Bow { get; set; }
    public double Skew { get; set; }
  }
}
