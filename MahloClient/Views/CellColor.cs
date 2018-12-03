using System;
using System.Drawing;
using MahloService.Settings;

namespace MahloClient.Views
{
  internal struct CellColor
  {
    public Color ForeColor { get; set; }

    public Color BackColor { get; set; }

    public void Deconstruct(out Color foreColor, out Color backColor)
    {
      foreColor = this.ForeColor;
      backColor = this.BackColor;
    }

    // Greenish
    public static readonly CellColor GoodColor = new CellColor { ForeColor = Color.FromArgb(0, 97, 0), BackColor = Color.FromArgb(198, 239, 206) };

    // Yellowish
    public static readonly CellColor ActiveColor = new CellColor { ForeColor = Color.FromArgb(156, 87, 0), BackColor = Color.FromArgb(255, 235, 156) };

    // Pinkish
    public static readonly CellColor OutOfSpecColor = new CellColor { ForeColor = Color.FromArgb(156, 0, 6), BackColor = Color.FromArgb(255, 199, 206) };

    // Gray
    public static readonly CellColor LimboColor = new CellColor { ForeColor = Color.White, BackColor = Color.FromArgb(165, 165, 165) };

    // Orangeish
    // public static readonly CellColor LimboColor = new CellColor { ForeColor = Color.FromArgb(63, 114, 142), BackColor = Color.FromArgb(255, 204, 153) };

    public static CellColor GetLimboColor()
    {
      return LimboColor;
    }

    public static CellColor GetFeetColor(int expectedLength, long measuredLength, IServiceSettings settings)
    {
      return
        measuredLength < expectedLength * settings.RollTooShortFactor ? OutOfSpecColor :
        measuredLength > expectedLength * settings.RollTooLongFactor ? OutOfSpecColor :
        GoodColor;
    }

    public static CellColor GetBowColor(string backingCode, double value, IServiceSettings settings)
    {
      var backingSpec = settings.GetBackingSpec(backingCode);
      return Math.Abs(value) > backingSpec.MaxBow ? OutOfSpecColor : GoodColor;
    }

    public static CellColor GetSkewColor(string backingCode, double value, IServiceSettings settings)
    {
      var backingSpecs = settings.GetBackingSpec(backingCode);
      return Math.Abs(value) > backingSpecs.MaxSkew ? OutOfSpecColor : GoodColor;
    }
  }
}
