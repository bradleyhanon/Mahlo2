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
    //private BehaviorSubject<ICriticalStops<Model>> changes;
    private IMeterSrc<Model> meterSrc;
    private bool isStatusIndicatorSet;

    public CriticalStops(IMeterSrc<Model> meterSrc)
    {
      this.meterSrc = meterSrc;
      //this.changes = new BehaviorSubject<ICriticalStops<Model>>(this);
    }

    [Flags]
    private enum Stop
    {
      MahloCommError = 1,
      PLCCommError = 2
    }

    //public IObservable<ICriticalStops<Model>> Changes => this.changes.AsObservable();

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

    public void Clear()
    {
      throw new NotImplementedException();
    }

    private void SetCriticalStops(Stop bitMask, bool value)
    {
      var oldStops = this.stops;
      this.stops = value ? this.stops |= bitMask : this.stops &= ~bitMask;
      if (this.Any != this.isStatusIndicatorSet)
      {
        this.isStatusIndicatorSet = this.Any;
        //this.changes.OnNext(this);

        //cmdWaitForSeam.Enabled = (nCriticalStops == 0);
        this.meterSrc.SetCriticalAlarm(this.isStatusIndicatorSet);
      }
    }
  }
}
