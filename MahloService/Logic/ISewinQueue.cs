using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Logic
{
  interface ISewinQueue : INotifyPropertyChanged, IDisposable
  {
    event Action QueueChanged;

    BindingList<CarpetRoll> Rolls { get; }

    string Message { get; }

    Task Refresh();

    void MoveRoll(int rollIndex, int direction);
  }
}