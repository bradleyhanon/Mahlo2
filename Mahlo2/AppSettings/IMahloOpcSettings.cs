namespace Mahlo.AppSettings
{
  interface IMahloOpcSettings
  {
    string Mahlo2ChannelName { get; set; }
    string BowAndSkewChannelName { get; set; }
    string PatternRepeatChannelName { get; set; }
  }
}