using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Opc;

namespace Mahlo.Logic
{
  class CriticalStops<Model> : ICriticalStops<Model>
  {
    private Stop stops;
    private BehaviorSubject<ICriticalStops<Model>> changes;

    public CriticalStops()
    {
      this.changes = new BehaviorSubject<ICriticalStops<Model>>(this);
    }

    [Flags]
    private enum Stop
    {
      MahloCommError = 1,
      PLCCommError = 2
    }

    public IObservable<ICriticalStops<Model>> Changes => this.changes.AsObservable();

    public bool Any => this.stops != 0;

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

    public IMeterSrc<Model> MeterSrc { get; set; }

    public void Clear()
    {
      throw new NotImplementedException();
    }

    private void SetCriticalStops(Stop value, bool off)
    {
      var oldStops = this.stops;
      this.stops = off ? this.stops &= ~value : this.stops |= value;
      if (this.stops != oldStops)
      {
        this.changes.OnNext(this);

        //cmdWaitForSeam.Enabled = (nCriticalStops == 0);
        this.MeterSrc.SetCriticalAlarm(this.stops != 0);
      }
    }
  }
}
