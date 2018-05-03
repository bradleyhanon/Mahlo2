using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace Mahlo.Logic
{
  [AddINotifyPropertyChangedInterface]
  class CriticalStops<Model> : ICriticalStops<Model>
  {
    private Stop stops;

    [Flags]
    private enum Stop
    {
      MahloCommError = 1,
      PLCCommError = 2
    }

    public bool Any { get; private set; }

    public bool IsMahloCommError
    {
      get => (this.stops & Stop.MahloCommError) != 0;
      set => this.SetCriticalStops(Stop.MahloCommError, value);
    }

    public bool IsPlcCommError
    {
      get => (this.stops & Stop.PLCCommError) != 0;
      set => this.SetCriticalStops(Stop.PLCCommError, value);
    }

    private void SetCriticalStops(Stop bitMask, bool value)
    {
      var oldStops = this.stops;
      this.stops = value ? this.stops |= bitMask : this.stops &= ~bitMask;
      this.Any = this.stops != 0;
    }
  }
}
