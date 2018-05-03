using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;
using Newtonsoft.Json;

namespace Mahlo.Logic
{
  interface IModelLogic
  {
    bool IsChanged { get; set; }
    CarpetRoll CurrentRoll { get; }
    [JsonProperty]
    string CurrentRollNo { get; }
    IObservable<CarpetRoll> RollStarted { get; }
    IObservable<CarpetRoll> RollFinished { get; }

    bool IsManualMode { get; }
    string Recipe { get; }

    IUserAttentions UserAttentions { get; set; }
    ICriticalStops CriticalStops { get; set; }

    void RefreshStatusDisplay();
    void MoveToNextRoll();
    void MoveToPriorRoll();
    void WaitForSeam();

    void Start();
  }
}
