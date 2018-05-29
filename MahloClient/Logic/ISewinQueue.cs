using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloClient.Logic
{
  interface ISewinQueue
  {
    event EventHandler Changed;

    BindingList<CarpetRoll> Rolls { get; }

    void UpdateSewinQueue(IEnumerable<CarpetRoll> rolls);
  }
}
