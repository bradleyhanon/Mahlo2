using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.Utilities.Configuration;

namespace Mahlo.AppSettings
{
  class PlcSettings : AppConfiguration, IPlcSettings
  {
    public PlcSettings()
    {
      this.Initialize();
    }

    public string ServerUri { get; set; } = "opc.tcp://127.0.0.1:49320";
    public string ChannelName { get; set; }

    public string Mahlo2SeamDetectTag { get; set; }
    public string Mahlo2SeamResetTag { get; set; }

    public string BowAndSkewSeamDetectTag { get; set; }
    public string BowAndSkewSeamResetTag { get; set; }

    public string PatternRepeatSeamDetectTag { get; set; }
    public string PatternRepeatSeamResetTag { get; set; }

    // Distance within which seam detects are ignored
    public double SeamDetectableThreshold { get; set; } = 5;
  }
}
