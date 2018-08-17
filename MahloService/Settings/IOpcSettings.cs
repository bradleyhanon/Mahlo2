namespace MahloService.Settings
{
  interface IOpcSettings
  {
    string OpcServerName { get; set; }
    string OpcServerPath { get; set; }
    string Mahlo2ChannelName { get; set; }
    string BowAndSkewChannelName { get; set; }
    string PatternRepeatChannelName { get; set; }
  }
}