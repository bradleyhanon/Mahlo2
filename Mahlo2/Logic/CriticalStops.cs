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
  class CriticalStops
  {
    private CriticalStopEnum nCriticalStops;
    private Subject<bool> mahloStatus = new Subject<bool>();
    private Subject<bool> plcStatus = new Subject<bool>();

    public IMahloSrc MahloSrc { get; set; }

    public bool Any => this.nCriticalStops != 0;

    public bool IsMahloCommError
    {
      get => (this.nCriticalStops & CriticalStopEnum.stpMahloCommError) != 0;
      set
      {
        if (IsMahloCommError != value)
        {
          this.SetCriticalStops(CriticalStopEnum.stpMahloCommError, value);
          this.mahloStatus.OnNext(value);
        }
      }
    }

    public bool IsPlcCommError
    {
      get => (this.nCriticalStops & CriticalStopEnum.stpPLCCommError) != 0;
      set
      {
        if (this.IsPlcCommError != value)
        {
          this.SetCriticalStops(CriticalStopEnum.stpPLCCommError, value);
          this.plcStatus.OnNext(value);
        }
      }
    }

    public IObservable<bool> PlcStatus => this.plcStatus.AsObservable();
    public IObservable<bool> MahloStatus => this.mahloStatus.AsObservable();

    private void SetCriticalStops(CriticalStopEnum StopValue, bool Off)
    {
      if (Off)
      {
        this.nCriticalStops &= ~StopValue;
      }
      else
      {
        if ((nCriticalStops & StopValue) != StopValue)
        {
          nCriticalStops |= StopValue;
        }
      }

      //cmdWaitForSeam.Enabled = (nCriticalStops == 0);
      this.MahloSrc.SetCriticalAlarm(this.nCriticalStops != 0);
    }
  }
}
