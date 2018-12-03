using Dapper.Contrib.Extensions;

namespace MahloService.Models
{
  [Table("PatternRepeatMap")]
  internal class PatternRepeatMapDatum
  {
    [ExplicitKey]
    public long FeetCounter { get; set; }
    public double Elongation { get; set; }
  }
}
