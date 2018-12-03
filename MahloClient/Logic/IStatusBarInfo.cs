using MahloService.Logic;

namespace MahloClient.Logic
{
  internal interface IStatusBarInfo
  {
    string ConnectionStatusMessage { get; }
    bool IsSeamDetectEnabled { get; }
    bool IsSeamDetected { get; }
    IUserAttentions UserAttentions { get; }
    ICriticalStops CriticalStops { get; }
    string AlertMessage { get; }
    string CriticalAlarmMessage { get; }
    bool IgnoringSeams { get; }
    string QueueMessage { get; }
  }
}
