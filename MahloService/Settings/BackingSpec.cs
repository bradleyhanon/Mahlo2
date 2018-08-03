using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MahloService.Settings
{
  public class BackingSpec
  {
    public string Backing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the maximum bow in inches.
    /// </summary>
    public double MaxBow { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the maximum skew in inches.
    /// </summary>
    public double MaxSkew { get; set; }

    /// <summary>
    /// Gets or seta a value indicating the maximum pattern elongation in percent.
    /// </summary>
    public double MaxElongation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the factor the DLot table is derived from.
    /// </summary>
    public double DlotSpec { get; set; }

    public static BackingSpec FromString(string s)
    {
      return JsonConvert.DeserializeObject<BackingSpec>(s);
    }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this).Replace('"', '\'');
    }
  }
}
