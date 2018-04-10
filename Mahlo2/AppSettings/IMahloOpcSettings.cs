namespace Mahlo.AppSettings
{
  interface IMahloOpcSettings
  {
    string ServerUri { get; set; }
    string Mahlo2ChannelName { get; set; }
    string BowAndSkewChannelName { get; set; }
    string PatternRepeatChannelName { get; set; }
  }
}