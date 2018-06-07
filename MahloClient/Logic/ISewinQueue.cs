using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using Newtonsoft.Json.Linq;

namespace MahloClient.Logic
{
  interface ISewinQueue
  {
    event EventHandler Changed;

    BindingList<CarpetRoll> Rolls { get; }

    void UpdateSewinQueue(JArray jsonRolls);
  }
}
