using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using PropertyChanged;

namespace MahloService.Models
{
  [AddINotifyPropertyChangedInterface]
  class CutRoll
  {
    public int Id { get; set; }
    public int GreigeRollId { get; set; }
    public string SapRoll { get; set; } = string.Empty;
    public int FeetCounterStart { get; set; }
    public int FeetCounterEnd { get; set; }

    [DependsOn(nameof(FeetCounterStart), nameof(FeetCounterEnd))]
    [Computed]
    [JsonIgnore]
    public int Length => this.FeetCounterEnd - this.FeetCounterStart;

    public double MaxBow { get; set; }
    public double MaxSkew { get; set; }
    public double MaxEPE { get; set; }
    public string Dlot { get; set; } = string.Empty;
  }
}
