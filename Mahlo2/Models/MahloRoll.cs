using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace Mahlo.Models
{
  [AddINotifyPropertyChangedInterface]
  class MahloRoll
  {
    public int Id { get; set; }

    public int Feet { get; set; }

    public virtual MahloRoll Create()
    {
      return new MahloRoll();
    }
  }
}