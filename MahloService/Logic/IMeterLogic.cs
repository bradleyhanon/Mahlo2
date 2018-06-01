using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using Newtonsoft.Json;

namespace MahloService.Logic
{
  interface IMeterLogic
  {
    event Action<CarpetRoll> RollStarted;
    event Action<CarpetRoll> RollFinished;

    [JsonIgnore]
    bool IsChanged { get; set; }
    [JsonIgnore]
    CarpetRoll CurrentRoll { get; set; }
    int CurrentRollIndex { get; }

    bool IsMappingNow { get; set; }
    bool IsManualMode { get; set; }

    string Recipe { get; set; }

    IUserAttentions UserAttentions { get; }
    ICriticalStops CriticalStops { get; }

    bool IsSeamDetected { get; set; }

    int MeasuredLength { get; set; }
    int Speed { get; set; }
    bool IsMapValid { get; set; }
    double MeasuredWidth { get; set; }

    int PreviousRollLength { get; set; }
    int RollChangesUntilCheckRequired { get; set; }
    int StyleChangesUntilCheckRequired { get; set; }

    string QueueMessage { get; set; }

    Task ApplyRecipe(string recipeName, bool isManualMode);
    void RefreshStatusDisplay();
    void MoveToNextRoll();
    void MoveToPriorRoll();
    void WaitForSeam();
    void DisableSystem();
    void Start();
  }

  interface IMeterLogic<Model> : IMeterLogic
  {
  }
}
