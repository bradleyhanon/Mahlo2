using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using PropertyChanged;

namespace Mahlo.Models
{
  [AddINotifyPropertyChangedInterface]
  public class CarpetRoll : IMahloRoll, IBowAndSkewRoll, IPatternRepeatRoll
  {
    public const string CheckRollId = "CHKROL";

    [ExplicitKey]
    public int RollId { get; set; }
    public string RollNo { get; set; }
    public string StyleCode { get; set; }
    public string StyleName { get; set; }
    public string ColorCode { get; set; }
    public string ColorName { get; set; }
    public string BackingCode { get; set; }
    public int RollLength { get; set; }
    public double RollWidth { get; set; }

    [DependsOn(nameof(RollWidth))]
    public string RollWidthStr => $"{(int)RollWidth / 12}' {(int)RollWidth % 12}\"";

    public string DefaultRecipe { get; set; }
    public decimal PatternRepeatLength { get; set; }
    public string ProductImageURL { get; set; }

    public int MalFeet { get; set; }
    public int BasFeet { get; set; }
    public int PrsFeet { get; set; }
    public int MalSpeed { get; set; }
    public int BasSpeed { get; set; }
    public int PrsSpeed { get; set; }
    public double Bow { get; set; }
    public double Skew { get; set; }
    public double Elongation { get; set; }

    public bool IsCheckRoll => this.RollNo == CheckRollId;

    int IMahloRoll.Feet
    {
      get => this.MalFeet;
      set => this.MalFeet = value;
    }

    int IBowAndSkewRoll.Feet
    {
      get => this.BasFeet;
      set => this.BasFeet = value;
    }
    int IPatternRepeatRoll.Feet
    {
      get => this.PrsFeet;
      set => this.PrsFeet = value;
    }

    int IMahloRoll.Speed
    {
      get => this.MalSpeed;
      set => this.MalSpeed = value;
    }

    int IBowAndSkewRoll.Speed
    {
      get => this.BasSpeed;
      set => this.BasSpeed = value;
    }

    int IPatternRepeatRoll.Speed
    {
      get => this.PrsSpeed;
      set => this.PrsSpeed = value;
    }

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
  }
}
