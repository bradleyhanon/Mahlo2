using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloClient.AppSettings
{
  public class ClientSettings : Westwind.Utilities.Configuration.AppConfiguration, IClientSettings
  {
    public ClientSettings()
    {
      this.Initialize();
    }

    public string ServiceUrl { get; set; } = "http://127.0.0.1/mahlo";

  }
}
