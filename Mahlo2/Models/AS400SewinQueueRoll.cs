using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Models
{
  class AS400SewinQueueRoll
  {
    public string GridImage { get; set; }
    public string G2ROLL { get; set; }
    public string G2SCH { get; set; }
    public int G2LTF { get; set; }
    public string G2STYL { get; set; }
    public string F2SDSC { get; set; }
    public string G2CLR { get; set; }
    public string F2CDSC { get; set; }
    public string G2SBK { get; set; }
    public string G2WTF { get; set; }
    public int G2WTI { get; set; }
    public string DefaultRecipe { get; set; }
    public decimal G2RPLN { get; set; }
    public int G2SJUL { get; set; }
    public string G2STME { get; set; }
    public string ProductImageURL { get; set; }

    public double RollLength => this.G2LTF;

    public string StyleCode => G2STYL;

    /// <summary>
    /// Copy all but RollId to the destination
    /// </summary>
    /// <param name="dest"></param>
    public CarpetRoll ToCarpetRoll()
    {
      CarpetRoll dest = new CarpetRoll
      {
        RollNo = this.G2ROLL.Trim(),
        StyleCode = this.G2STYL.Trim(),
        StyleName = this.F2SDSC.Trim(),
        ColorCode = this.G2CLR.Trim(),
        ColorName = this.F2CDSC.Trim(),
        BackingCode = this.G2SBK.Trim(),
        PatternRepeatLength = (double)this.G2RPLN,

        RollLength = this.G2LTF,
        ProductImageURL = this.ProductImageURL?.Trim() ?? string.Empty,
      };

      if (double.TryParse(this.G2WTF, out double wtf))
      {
        dest.RollWidth = wtf * 12 + this.G2WTI;
      }

      //dest.G2SJUL = this.G2SJUL;
      //dest.G2STME = this.G2STME;

      return dest;
    }
  }
}
