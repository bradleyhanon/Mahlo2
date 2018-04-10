using System.Collections.ObjectModel;
using Mahlo.Models;

namespace Mahlo.Logic
{
  interface ISewinQueue
  {
    ObservableCollection<GreigeRoll> Rolls { get; }

    bool RollIsLeader(int currentRollId);
  }
}