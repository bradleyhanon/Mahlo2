using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Logic
{
  internal interface ISewinQueue : IDisposable
  {
    event CancelEventHandler CanRemoveRollQuery;

    event Action QueueChanged;

    bool IsChanged { get; set; }

    BindingList<GreigeRoll> Rolls { get; }

    string Message { get; }

    Task RefreshAsync();

    void MoveRoll(int rollIndex, int direction);
  }
}