using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using Microsoft.AspNet.SignalR.Client;

namespace MahloClient.Logic
{
  interface IStatusBarInfo
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
