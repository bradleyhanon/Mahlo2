using System;
using Dapper.Contrib.Extensions;
using PropertyChanged;

namespace MahloService.Models
{
  public enum RollTypeEnum
  {
    Greige,
    Finished,
    Leader,
    CheckRoll
  }

  [AddINotifyPropertyChangedInterface]
  public class GreigeRoll
  {
    public const string CheckRollId = "CHKROL";

    [ExplicitKey]
    public int Id { get; set; }
    public bool IsComplete { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the roll has been removed from the queue but is still in use.
    /// </summary>
    [Computed]
    public bool IsInLimbo { get; set; }

    //-- Mfg database data --//
    public string RollNo { get; set; } = string.Empty;
    public string OrderNo { get; set; } = string.Empty;
    public string StyleCode { get; set; } = string.Empty;
    public string StyleName { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public string ColorName { get; set; } = string.Empty;
    public string BackingCode { get; set; } = string.Empty;
    public int RollLength { get; set; }
    public double RollWidth { get; set; }

    [DependsOn(nameof(RollWidth))]
    [Computed]
    public string RollWidthStr => CommonMethods.InchesToStr(this.RollWidth);

    public string DefaultRecipe { get; set; } = string.Empty;
    public double PatternRepeatLength { get; set; }
    public string ProductImageURL { get; set; }

    // Runtime data
    public long MalFeetCounterStart { get; set; }
    public long MalFeetCounterEnd { get; set; }
    [DependsOn(nameof(MalFeetCounterStart), nameof(MalFeetCounterEnd))]
    [Computed]
    public long MalFeet => this.MalFeetCounterEnd - this.MalFeetCounterStart;
    public int MalSpeed { get; set; }
    public bool MalMapValid { get; set; }

    public long BasFeetCounterStart { get; set; }
    public long BasFeetCounterEnd { get; set; }
    [DependsOn(nameof(BasFeetCounterEnd), nameof(BasFeetCounterEnd))]
    [Computed]
    public long BasFeet => this.BasFeetCounterEnd - this.BasFeetCounterStart;
    public int BasSpeed { get; set; }
    public bool BasMapValid { get; set; }

    public long PrsFeetCounterStart { get; set; }
    public long PrsFeetCounterEnd { get; set; }
    [DependsOn(nameof(PrsFeetCounterStart), nameof(PrsFeetCounterEnd))]
    [Computed]
    public long PrsFeet => this.PrsFeetCounterEnd - this.PrsFeetCounterStart;
    public int PrsSpeed { get; set; }
    public bool PrsMapValid { get; set; }

    public double Bow { get; set; }
    public double Skew { get; set; }

    public double Elongation { get; set; }

    [DependsOn(nameof(RollNo))]
    [Computed]
    public bool IsCheckRoll => this.RollNo == CheckRollId;

    /// <summary>
    /// Copy all but RollId to the destination
    /// </summary>
    /// <param name="dest"></param>
    public void CopyTo(GreigeRoll dest)
    {
      if (dest == null)
      {
        throw new NullReferenceException();
      }

      //dest.GridImage = this.GridImage;
      dest.RollNo = this.RollNo;
      dest.OrderNo = this.OrderNo;
      dest.StyleCode = this.StyleCode;
      dest.StyleName = this.StyleName;
      dest.ColorCode = this.ColorCode;
      dest.ColorName = this.ColorName;
      dest.BackingCode = this.BackingCode;
      dest.RollLength = this.RollLength;
      dest.RollWidth = this.RollWidth;
      dest.DefaultRecipe = this.DefaultRecipe;
      dest.PatternRepeatLength = this.PatternRepeatLength;
      dest.ProductImageURL = this.ProductImageURL;
    }

    public void SwapWith(GreigeRoll other)
    {
      (other.RollNo, this.RollNo) = (this.RollNo, other.RollNo);
      (other.OrderNo, this.OrderNo) = (this.OrderNo, other.OrderNo);
      (other.StyleCode, this.StyleCode) = (this.StyleCode, other.StyleCode);
      (other.StyleName, this.StyleName) = (this.StyleName, other.StyleName);
      (other.ColorCode, this.ColorCode) = (this.ColorCode, other.ColorCode);
      (other.ColorName, this.ColorName) = (this.ColorName, other.ColorName);
      (other.BackingCode, this.BackingCode) = (this.BackingCode, other.BackingCode);
      (other.RollLength, this.RollLength) = (this.RollLength, other.RollLength);
      (other.RollWidth, this.RollWidth) = (this.RollWidth, other.RollWidth);
      (other.DefaultRecipe, this.DefaultRecipe) = (this.DefaultRecipe, other.DefaultRecipe);
      (other.PatternRepeatLength, this.PatternRepeatLength) = (this.PatternRepeatLength, other.PatternRepeatLength);
      (other.ProductImageURL, this.ProductImageURL) = (this.ProductImageURL, other.ProductImageURL);
    }

    public override string ToString()
    {
      return $"RollNo={this.RollNo}, RollWidth={this.RollWidth}";
    }
  }
}
