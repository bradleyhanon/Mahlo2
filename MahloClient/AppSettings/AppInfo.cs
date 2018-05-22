using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloClient.AppSettings
{
  public class AppInfo : Westwind.Utilities.Configuration.AppConfiguration, IAppInfo
  {
    public AppInfo()
    {
      this.Initialize();
    }

    public string ServerUrl { get; set; } = "http://127.0.0.1/mahlo";

  }
}
