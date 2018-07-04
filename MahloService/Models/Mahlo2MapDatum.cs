using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace MahloService.Models
{
  [Table("Mahlo2Map")]
  class Mahlo2MapDatum
  {
    [ExplicitKey]
    public long FeetCounter { get; set; }
  }
}
