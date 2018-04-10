using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using PropertyChanged;

namespace Mahlo.Models
{
  [AddINotifyPropertyChangedInterface]
  class MahloRoll
  {
    [ExplicitKey]
    public int RollId { get; set; }

    [DependsOn(nameof(Meters))]
    public double Feet => Extensions.MetersToFeet(this.Meters);

    public double Meters { get; set; }

    public virtual MahloRoll Create()
    {
      return new MahloRoll();
    }
  }
}