using System;

namespace MahloClient.Views
{
  static class MyColors
  {
    //// Greenish
    //public static readonly Color GoodBackColor = Color.FromArgb(198, 239, 206);
    //public static readonly Color GoodForeColor = Color.FromArgb(0, 97, 0);

    //// Yellowish
    //public static readonly Color ActiveBackColor = Color.FromArgb(255, 235, 156);
    //public static readonly Color ActiveForeColor = Color.FromArgb(156, 87, 0);

    //// Pinkish background
    //public static readonly Color OutOfSpecBackColor = Color.FromArgb(255, 199, 206);
    //public static readonly Color OutOfSpecForeColor = Color.FromArgb(156, 0, 6);

    // Greenish
    public static (Color foreColor, Color backColor) GoodColor = (Color.FromArgb(198, 239, 206), Color.FromArgb(0, 97, 0));

    // Yellowish
    public static (Color foreColor, Color backColor) ActiveColor = (Color.FromArgb(255, 235, 156), Color.FromArgb(156, 87, 0));

    // Pinkish
    public static (Color foreColor, Color backColor) OutOfSpecColor = (Color.FromArgb(255, 199, 206), Color.FromArgb(156, 0, 6));

    public static void SetFeetColor(DataGridView grid, DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)grid.Rows[args.RowIndex].DataBoundItem;
      long measuredLength = (long)args.Value;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        roll.RollLength == 0 ? (grid.DefaultCellStyle.BackColor, grid.DefaultCellStyle.ForeColor) :
        measuredLength < roll.RollLength * settings.RollTooShortFactor ? OutOfSpecColor :
        measuredLength > roll.RollLength * settings.RollTooLongFactor ? OutOfSpecColor :
        GoodColor;
    }

    public static void SetBowColor(DataGridView grid, DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)grid.Rows[args.RowIndex].DataBoundItem;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        (roll.BackingCode == "SA" && Math.Abs((double)args.Value) > settings.BowLimitSA) ||
        (roll.BackingCode != "SA" && Math.Abs((double)args.Value) > settings.BowLimitVinyl) ?
        OutOfSpecColor : GoodColor;
    }

    public static void SetSkewColor(DataGridView grid, DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {
      GreigeRoll roll = (GreigeRoll)grid.Rows[args.RowIndex].DataBoundItem;

      (args.CellStyle.BackColor, args.CellStyle.ForeColor) =
        (roll.BackingCode == "SA" && Math.Abs((double)args.Value) > settings.SkewLimitSA) ||
        (roll.BackingCode != "SA" && Math.Abs((double)args.Value) > settings.SkewLimitVinyl) ?
        OutOfSpecColor : GoodColor;
    }

    public static void SetElongationColor(DataGridView grid, DataGridViewCellFormattingEventArgs args, IServiceSettings settings)
    {

    }
  }
}