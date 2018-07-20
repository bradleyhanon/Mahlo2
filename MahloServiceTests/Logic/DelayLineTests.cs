using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using Xunit;

namespace MahloServiceTests.Logic
{
  public class DelayLineTests
  {
    private const int retain = 20;
    private const int distance = 15;

    private static int currentTick = new Random().Next();

    private readonly DelayLine<bool> target = new DelayLine<bool>
    {
      RetainTicks = retain,
      DelayTicks = distance,
    };

    [Fact]
    public void DelaysOneItem()
    {
      Assert.True(this.Forward(distance, true));
      Assert.True(this.target.Value);
    }

    [Fact]
    public void DelaysTwoValues()
    {
      Assert.False(this.Forward(5, true));
      Assert.False(this.Forward(5, false));
      Assert.True(this.Forward(distance - 10, false));
      Assert.True(this.target.Value);

      Assert.True(this.Forward(5, false));
      Assert.False(this.target.Value);
    }

    [Fact]
    public void ReverseMotionSeesDelayedValuesAgain()
    {
      DelaysTwoValues();

      Assert.True(this.Reverse(1, false));
      Assert.True(this.target.Value);

      Assert.True(this.Reverse(5, false));
      Assert.False(this.target.Value);
    }

    [Fact]
    public void ValuesAreNotAvailableForReverseWhenRetentionIsExceeded()
    {
      Assert.True(this.Forward(distance, true));

      // It works at the retain limit
      Assert.False(this.Forward(retain, true));
      Assert.False(this.Reverse(retain, true));
      Assert.True(this.Reverse(1, true));
      Assert.True(this.Forward(1, true));
      Assert.True(this.target.Value);

      // It doesn't work beyond the retain limit.
      Assert.True(this.Forward(retain + 1, true));
      Assert.False(this.Reverse(retain + 1, true));
      Assert.False(this.Reverse(1, true));
      Assert.False(this.Forward(1, true));
      Assert.False(this.target.Value);
    }

    private bool Forward(object delayTicks, bool v)
    {
      throw new NotImplementedException();
    }

    private bool Forward(int ticks, bool value)
    {
      for (int j = 0; j < ticks; j++)
      {
        Assert.False(this.target.Add(currentTick, value));
        currentTick++;
      }

      return this.target.Add(currentTick, value);
    }

    private bool Reverse(int ticks, bool value)
    {
      for (int j = 0; j < ticks; j++)
      {
        Assert.False(this.target.Add(currentTick, value));
        currentTick--;
      }

      return this.target.Add(currentTick, value);
    }
  }
}
