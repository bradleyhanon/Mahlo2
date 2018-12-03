using System;
using System.Collections.Generic;
using System.Linq;
using MahloService.Repository;

namespace MahloService.Logic
{
  internal class DelayLine<T>
    where T : IEquatable<T>
  {
    private List<DelayItem> delayItems = new List<DelayItem>();


    /// <summary>
    /// Gets or sets a value indicating the destance between seam detector and
    /// where end of roll would be logically be expected to be.
    /// </summary>
    public double DelayTicks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how long delay items should be kept
    /// beyond DelayedValue to allow for reversal
    /// </summary>
    public double RetainTicks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the number of ticks to delay the value
    /// </summary>
    public T Value { get; private set; }

    public void SaveState(IProgramState state)
    {
      state = state.GetSubState(nameof(DelayLine<T>));
      state.Set(nameof(this.delayItems), this.delayItems);
    }

    public void RestoreState(IProgramState state)
    {
      state = state.GetSubState(nameof(DelayLine<T>));
      this.delayItems = state.Get<List<DelayItem>>(nameof(this.delayItems)) ?? new List<DelayItem>();
    }

    /// <summary>
    /// Add an observance of the value
    /// </summary>
    /// <param name="tick">The tick at whch the value was seen.</param>
    /// <param name="value">The value that was seen.</param>
    /// <returns>True if the delay value changed.</returns>
    public bool Add(double tick, T value)
    {
      // Remove expired values
      while (this.delayItems.Count > 0 &&
        tick - this.delayItems[0].Tick > this.DelayTicks + this.RetainTicks)
      {
        this.delayItems.RemoveAt(0);
      }

      while (true)
      {
        var lastItem = this.delayItems.LastOrDefault();
        if (lastItem == null)
        {
          if (!value.Equals(default(T)))
          {
            this.Append(tick, value);
          }
        }
        else
        {
          // Handle reverse motion
          if (tick < lastItem.Tick)
          {
            this.delayItems.RemoveAt(this.delayItems.Count - 1);
            continue;
          }

          // Add new item if value has changed
          if (!lastItem.Value.Equals(value))
          {
            this.Append(tick, value);
          }
        }

        break;
      }

      return this.CheckForChange(tick);
    }


    private bool CheckForChange(double tick)
    {
      bool changed = false;
      var item = this.delayItems.LastOrDefault(x => tick - x.Tick >= this.DelayTicks);
      T newValue = item == null ? default(T) : item.Value;
      if (!newValue.Equals(this.Value))
      {
        this.Value = newValue;
        changed = true;
      }

      return changed;
    }

    private void Append(double tick, T value)
    {
      this.delayItems.Add(new DelayItem { Tick = tick, Value = value });
    }

    private class DelayItem
    {
      public double Tick { get; set; }
      public T Value { get; set; }
    }
  }
}
