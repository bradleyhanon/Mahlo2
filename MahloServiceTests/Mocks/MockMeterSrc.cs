using System;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Opc;
using PropertyChanged;

namespace MahloServiceTests.Mocks
{
  [AddINotifyPropertyChangedInterface]
  internal class MockMeterSrc<Model> : IMahloSrc, IBowAndSkewSrc, IPatternRepeatSrc
  {
    public double FeetCounter { get; set; }
    public double FeetPerMinute { get; set; }
    public double MeasuredWidth { get; set; }
    public double BowInInches { get; set; }
    public double SkewInInches { get; set; }
    public double PatternRepeatLength { get; set; }
    public bool IsSeamDetected { get; set; }
    public bool IsDoffDetected { get; set; }

    public string Recipe { get; set; } = string.Empty;
    public bool IsAutoMode { get; set; }


    public int ResetMeterOffsetCalled { get; set; }
    public int AcknowledgeSeamDetectCalled { get; set; }
    public int AcknowledgeDoffDetectCalled { get; set; }

    public void ResetMeterOffset()
    {
      this.ResetMeterOffsetCalled++;
    }

    public Task AcknowledgeSeamDetectAsync(CancellationToken token)
    {
      this.AcknowledgeSeamDetectCalled++;
      return Task.CompletedTask;
    }

    public Task AcknowledgeDoffDetectAsync(CancellationToken token)
    {
      this.AcknowledgeDoffDetectCalled++;
      return Task.CompletedTask;
    }

    public void SetAutoMode(bool value)
    {
      throw new NotImplementedException();
    }

    public void SetCriticalAlarmIndicator(bool value)
    {
      throw new NotImplementedException();
    }

    public void SetMiscellaneousIndicator(bool value)
    {
      //throw new NotImplementedException();
    }

    public void SetRecipe(string recipeName)
    {
      throw new NotImplementedException();
    }

    public void SetStatusIndicator(bool value)
    {
      throw new NotImplementedException();
    }

    public void SetRecipeFromPatternLength(double targetPatternRepeatLength)
    {
      throw new NotImplementedException();
    }
  }
}
