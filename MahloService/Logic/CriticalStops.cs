using System;
using PropertyChanged;

namespace MahloService.Logic
{
  [AddINotifyPropertyChangedInterface]
  internal class CriticalStops<Model> : ICriticalStops<Model>
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
