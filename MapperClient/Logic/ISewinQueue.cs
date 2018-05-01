﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace MapperClient.Logic
{
  interface ISewinQueue
  {
    BindingList<CarpetRoll> Rolls { get; }

    void UpdateSewinQueue(IEnumerable<CarpetRoll> rolls);
  }
}
