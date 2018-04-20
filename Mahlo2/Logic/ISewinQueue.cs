﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface ISewinQueue : IDisposable
  {
    BindingList<CarpetRoll> Rolls { get; }

    //bool RollIsLeader(int currentRollId);

    /// <summary>
    /// Gets a Roll from the sewin queue.
    /// </summary>
    /// <param name="rollId">The RollId of the roll to get.</param>
    /// <returns>The requested roll, if it doesn't exist then the next roll, if it doesn't exist then a new roll.</returns>
    CarpetRoll GetRoll(int rollId);

    /// <summary>
    /// Try to get a roll from the sewin queue
    /// </summary>
    /// <param name="RollId">The Id of the roll to get.</param>
    /// <param name="roll">The roll gotten.</param>
    /// <returns>True if the roll was found, false if a dummy roll was returned.</returns>
    bool TryGetRoll(int RollId, out CarpetRoll roll);
  }
}