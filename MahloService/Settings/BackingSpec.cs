using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Settings
{
  public class BackingSpec
  {
    public string Backing { get; set; }
    public double MaxBow { get; set; }
    public double MaxSkew { get; set; }
    public double DlotSpec { get; set; }

    public override string ToString()
    {
      return $"{Backing}, {MaxBow}, {MaxSkew}, {DlotSpec}";
    }

    public static BackingSpec FromString(string s)
    {

      string[] ss = s.Split(',');
      if (ss.Length == 4 && 
        double.TryParse(ss[1].Trim(), out double maxBow) &&
        double.TryParse(ss[2].Trim(), out double maxSkew) &&
        double.TryParse(ss[3].Trim(), out double dlotSpec))
      {
        return new BackingSpec
        {
          Backing = ss[0].Trim().ToUpper(),
          MaxBow = maxBow,
          MaxSkew = maxSkew,
          DlotSpec = dlotSpec
        };
      }

      throw new Exception($"Bad EpeSpec in ServiceSettings: '{s}'");
    }
  }
}
