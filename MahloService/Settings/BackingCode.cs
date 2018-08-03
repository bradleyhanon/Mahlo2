using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MahloService.Settings
{
  public class BackingCode
  {
    public string Code { get; set; }
    public string Backing { get; set; }

    public static BackingCode FromString(string s)
    {
      return JsonConvert.DeserializeObject<BackingCode>(s);
    }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this).Replace('"', '\'');
    }
  }
}