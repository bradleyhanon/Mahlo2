using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using MahloService.Opc;

namespace MahloServiceTests.Mocks
{
  class MockMeterSrc<Model> : IMahloSrc<Model>, IBowAndSkewSrc<Model>, IPatternRepeatSrc<Model>
  {
    public Subject<int> FeetCounterSubject { get; } = new Subject<int>();
    public Subject<int> FeetPerMinuteSubject { get; } = new Subject<int>();
    public Subject<bool> SeamDetectedSubject { get; } = new Subject<bool>();
    public Subject<double> BowSubject { get; } = new Subject<double>();
    public Subject<double> SkewSubject { get; } = new Subject<double>();
    public Subject<double> PatternRepeatSubject { get; } = new Subject<double>();

    public IObservable<int> FeetCounter => this.FeetCounterSubject;

    public IObservable<int> FeetPerMinute => this.FeetPerMinuteSubject;

    public IObservable<bool> SeamDetected => this.SeamDetectedSubject;

    public string Recipe { get; set; } = string.Empty;
    public bool IsAutoMode { get; set; }

    public IObservable<double> BowChanged => this.BowSubject;

    public IObservable<double> SkewChanged => this.SkewSubject;

    public IObservable<double> PatternRepeatChanged => this.PatternRepeatSubject;

    public int ResetMeterOffsetCalled { get; set; }
    public int ResetSeamDetectorCalled { get; set; }

    public void ResetMeterOffset()
    {
      this.ResetMeterOffsetCalled++;
    }

    public void ResetSeamDetector()
    {
      this.ResetSeamDetectorCalled++;
    }

    public void SetAutoMode(bool value)
    {
      throw new NotImplementedException();
    }

    public void SetCriticalAlarm(bool value)
    {
      throw new NotImplementedException();
    }

    public void SetRecipe(string recipeName)
    {
      throw new NotImplementedException();
    }

    public void SetStatusIndicator(bool value)
    {
      throw new NotImplementedException();
    }
  }
}
