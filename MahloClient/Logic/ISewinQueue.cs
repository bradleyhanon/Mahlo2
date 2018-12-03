using System;
using System.ComponentModel;
using MahloService.Models;
using Newtonsoft.Json.Linq;

namespace MahloClient.Logic
{
  internal interface ISewinQueue
  {
    event EventHandler Changed;

    BindingList<GreigeRoll> Rolls { get; }

    void UpdateSewinQueue(JArray jsonRolls);
  }
}
