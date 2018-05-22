using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.Configuration;

namespace MahloService.AppSettings
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
  }
}
