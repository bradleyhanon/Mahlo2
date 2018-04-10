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
  }
}