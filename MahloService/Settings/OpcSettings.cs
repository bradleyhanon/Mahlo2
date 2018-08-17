using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.Configuration;

namespace MahloService.Settings
{
  class OpcSettings : AppConfiguration, IOpcSettings
  {
    public OpcSettings()
    {
      this.Initialize();
    }

    public string OpcServerName { get; set; } = "MFC-Anwendung 10AOpcServer";
    public string OpcServerPath { get; set; } = @"C:\Program Files (x86)\Mahlo\10AOpcServer\10AOpcServer.exe";

    public string Mahlo2ChannelName { get; set; } = "Simulator"; // = "BowAndSkew2";
    public string BowAndSkewChannelName { get; set; } = "Simulator"; // = "BowAndSkew3";
    public string PatternRepeatChannelName { get; set; } = "Simulator"; // = "PatternRepeat4";
  }
}
