namespace Mahlo.AppSettings
{
  interface IPlcSettings
  {
    string Mahlo2SeamDetectTag { get; set; }
    string Mahlo2SeamResetTag { get; set; }

    string BowAndSkewSeamDetectTag { get; set; }
    string BowAndSkewSeamResetTag { get; set; }

    string PatternRepeatSeamDetectTag { get; set; }
    string PatternRepeatSeamResetTag { get; set; }
  }
}