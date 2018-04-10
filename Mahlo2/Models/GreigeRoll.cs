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

    public bool IsCheckRoll => this.G2ROLL == CheckRollId;
    public double RollLength => this.G2LTF;

    public string StyleCode => G2STYL;

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

      dest.GridImage = this.GridImage;
      dest.G2ROLL = this.G2ROLL;
      dest.G2SCH = this.G2SCH;
      dest.G2LTF = this.G2LTF;
      dest.G2STYL = this.G2STYL;
      dest.F2SDSC = this.F2SDSC;
      dest.G2CLR = this.G2CLR;
      dest.F2CDSC = this.F2CDSC;
      dest.G2SBK = this.G2SBK;
      dest.G2WTF = this.G2WTF;
      dest.G2WTI = this.G2WTI;
      dest.DefaultRecipe = this.DefaultRecipe;
      dest.G2RPLN = this.G2RPLN;
      dest.G2SJUL = this.G2SJUL;
      dest.G2STME = this.G2STME;
      dest.ProductImageURL = this.ProductImageURL;
    }
  }
}
