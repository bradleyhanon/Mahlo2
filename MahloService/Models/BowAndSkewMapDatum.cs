using Dapper.Contrib.Extensions;

namespace MahloService.Models
{
  [Table("BowAndSkewMap")]
  internal class BowAndSkewMapDatum
  {
    [ExplicitKey]
    public long FeetCounter { get; set; }
    public double Bow { get; set; }
    public double Skew { get; set; }
  }
}
