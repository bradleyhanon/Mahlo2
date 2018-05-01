using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperClient.AppSettings
{
  public class AppInfo : Westwind.Utilities.Configuration.AppConfiguration, IAppInfo
  {
    public AppInfo()
    {
      this.Initialize();
    }

    public string ServerUrl { get; set; }

  }
}
