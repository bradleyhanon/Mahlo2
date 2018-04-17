using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface ISewinQueue : IDisposable
  {
    BindingList<GreigeRoll> Rolls { get; }

    bool RollIsLeader(int currentRollId);

    /// <summary>
    /// Gets a Roll from the sewin queue.
    /// </summary>
    /// <param name="rollId">The RollId of the roll to get.</param>
    /// <returns>The requested roll, if it doesn't exist then the next roll, if it doesn't exist then a new roll.</returns>
    GreigeRoll GetRoll(int rollId);
  }
}