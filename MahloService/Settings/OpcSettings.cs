using Westwind.Utilities.Configuration;

namespace MahloService.Settings
{
  internal class OpcSettings : AppConfiguration, IOpcSettings
  {
    public OpcSettings()
    {
      this.Initialize();
    }

    public string OpcServerPath { get; set; } = @"C:\Program Files (x86)\Mahlo\10AOpcServer\10AOpcServer.exe";

    public string Mahlo2ChannelName { get; set; } = "Simulator"; // = "BowAndSkew2";
    public string BowAndSkewChannelName { get; set; } = "Simulator"; // = "BowAndSkew3";
    public string PatternRepeatChannelName { get; set; } = "Simulator"; // = "PatternRepeat4";
  }
}
