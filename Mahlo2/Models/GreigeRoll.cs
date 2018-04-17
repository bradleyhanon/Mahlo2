using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace Mahlo.Models
{
  public class GreigeRoll
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

    public string DefaultRecipe { get; set; }
    public decimal PatternRepeatLength { get; set; }
    public string ProductImageURL { get; set; }

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
