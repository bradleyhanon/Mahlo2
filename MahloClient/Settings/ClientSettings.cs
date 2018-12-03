using System.Configuration;

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
