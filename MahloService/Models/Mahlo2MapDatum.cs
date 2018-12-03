using Dapper.Contrib.Extensions;

namespace MahloService.Models
{
  [Table("Mahlo2Map")]
  internal class Mahlo2MapDatum
  {
    [ExplicitKey]
    public long FeetCounter { get; set; }
  }
}
