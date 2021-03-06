﻿using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using PropertyChanged;

namespace MahloService.Models
{
  [AddINotifyPropertyChangedInterface]
  internal class CutRoll
  {
    [ExplicitKey]
    public int Id { get; set; }
    public int GreigeRollId { get; set; }
    public string SapRoll { get; set; } = string.Empty;
    public long FeetCounterStart { get; set; }
    public long FeetCounterEnd { get; set; }

    [DependsOn(nameof(FeetCounterStart), nameof(FeetCounterEnd))]
    [Computed]
    [JsonIgnore]
    public long Length => this.FeetCounterEnd - this.FeetCounterStart;

    public double Bow { get; set; }
    public double Skew { get; set; }
    public double EPE { get; set; }
    public string Dlot { get; set; } = string.Empty;
    public double Elongation { get; set; }
  }
}
