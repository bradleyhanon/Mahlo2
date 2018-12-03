namespace MahloService.Logic
{
  internal interface ICriticalStops
  {
    bool Any { get; }
    bool IsMahloCommError { get; set; }
    bool IsPlcCommError { get; set; }
  }

  internal interface ICriticalStops<Model> : ICriticalStops
  {
  }
}