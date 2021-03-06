﻿using System;
using System.Threading.Tasks;
using MahloService.Models;
using Newtonsoft.Json;

namespace MahloService.Logic
{
  internal interface IMeterLogic
  {
    event Action<GreigeRoll> RollStarted;
    event Action<GreigeRoll> RollFinished;

    [JsonIgnore]
    bool IsChanged { get; set; }
    [JsonIgnore]
    GreigeRoll CurrentRoll { get; set; }
    int CurrentRollIndex { get; }

    bool IsMappingNow { get; set; }
    bool IsManualMode { get; set; }

    string Recipe { get; set; }

    IUserAttentions UserAttentions { get; }
    ICriticalStops CriticalStops { get; }

    bool IsSeamDetected { get; set; }

    long MeasuredLength { get; }
    int Speed { get; set; }
    bool IsMapValid { get; set; }
    double MeasuredWidth { get; set; }

    int PreviousRollLength { get; set; }
    int RollChangesUntilCheckRequired { get; set; }
    int StyleChangesUntilCheckRequired { get; set; }

    string QueueMessage { get; set; }

    Task ApplyRecipeAsync(string recipeName, bool isManualMode);
    void RefreshStatusDisplay();
    void MoveToNextRoll(int lengthOfCurrentRoll);
    void MoveToPriorRoll();
    void WaitForSeam();
    void DisableSystem();
    void Start();
  }

  internal interface IMeterLogic<Model> : IMeterLogic
  {
  }
}
