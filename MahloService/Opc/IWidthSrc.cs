namespace MahloService.Opc
{
  internal interface IWidthSrc<Model> : IMeterSrc<Model>
  {
    bool OnOff { get; set; }
    int Status { get; set; }
    double MeterStamp { get; set; }
    bool Valid { get; set; }
    double ValueInMeter { get; set; }
  }
}
