using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;

namespace MahloClient.Views
{
  struct CellColor
  {
    public Color ForeColor { get; set; }

    public Color BackColor { get; set; }

    public void Deconstruct(out Color foreColor, out Color backColor)
    {
      foreColor = this.ForeColor;
      backColor = this.BackColor;
    }

    // Greenish
    public static readonly CellColor GoodColor = new CellColor { ForeColor = Color.FromArgb(198, 239, 206), BackColor = Color.FromArgb(0, 97, 0) };

    // Yellowish
    public static readonly CellColor ActiveColor = new CellColor { ForeColor = Color.FromArgb(255, 235, 156), BackColor = Color.FromArgb(156, 87, 0) };

    // Pinkish
    public static readonly CellColor OutOfSpecColor = new CellColor { ForeColor = Color.FromArgb(255, 199, 206), BackColor = Color.FromArgb(156, 0, 6) };

    public static CellColor GetFeetColor(int expectedLength, long measuredLength, IServiceSettings settings)
    {
      return
        measuredLength < expectedLength * settings.RollTooShortFactor ? OutOfSpecColor :
        measuredLength > expectedLength * settings.RollTooLongFactor ? OutOfSpecColor :
        GoodColor;
    }

    public static CellColor GetBowColor(string backingCode, double value, IServiceSettings settings)
    {
      double limit = backingCode.Equals("SA", StringComparison.InvariantCultureIgnoreCase) ?
        settings.BowLimitSA : settings.BowLimitVinyl;

      return Math.Abs(value) > limit ? OutOfSpecColor : GoodColor;
    }

    public static CellColor GetSkewColor(string backingCode, double value, IServiceSettings settings)
    {
      double limit = backingCode.Equals("SA", StringComparison.InvariantCultureIgnoreCase) ?
        settings.SkewLimitSA : settings.SkewLimitVinyl;

      return Math.Abs(value) > limit ? OutOfSpecColor : GoodColor;
    }
  }
}
