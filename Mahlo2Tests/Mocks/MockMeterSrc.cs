using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Opc;

namespace Mahlo2Tests.Mocks
{
  class MockMeterSrc : IMahloSrc
  {
    public Subject<int> FeetCounterSubject { get; } = new Subject<int>();
    public Subject<int> FeetPerMinuteSubject { get; } = new Subject<int>();
    public Subject<bool> SeamDetectedSubject { get; } = new Subject<bool>();
    public IObservable<int> FeetCounter => this.FeetCounterSubject;

    public IObservable<int> FeetPerMinute => this.FeetPerMinuteSubject;

    public IObservable<bool> SeamDetected => this.SeamDetectedSubject;

    public event PropertyChangedEventHandler PropertyChanged;

    public void ResetMeterOffset()
    {
      throw new NotImplementedException();
    }

    public void ResetSeamDetector()
    {
      throw new NotImplementedException();
    }

    public void SetCriticalAlarm(bool value)
    {
      throw new NotImplementedException();
    }

    public void SetStatusIndicator(bool value)
    {
      throw new NotImplementedException();
    }
  }
}
