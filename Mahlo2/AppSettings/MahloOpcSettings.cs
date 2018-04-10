using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.Configuration;

namespace Mahlo.AppSettings
{
  class MahloOpcSettings : AppConfiguration, IMahloOpcSettings
  {
    public MahloOpcSettings()
    {
      this.Initialize();
    }

    public string Mahlo2ChannelName { get; set; } = "Simulator"; // = "BowAndSkew2";
    public string BowAndSkewChannelName { get; set; } = "Simulator"; // = "BowAndSkew3";
    public string PatternRepeatChannelName { get; set; } = "Simulator"; // = "PatternRepeat4";

    public string ServerUri { get; set; } = "opc.tcp://127.0.0.1:843";
  }
}
