using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloClient.AppSettings
{
  public class ClientSettings : IClientSettings
  {
    public ClientSettings()
    {
      this.ServiceUrl = ConfigurationManager.AppSettings["ServerUrl"] ?? this.ServiceUrl;
    }

    public string ServiceUrl { get; set; } = "http://172.23.1.44/mahlo";
  }
}
