using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;

namespace MahloService.Simulation
{
  interface IDbMfgSim : IDbMfg
  {
    BindingList<GreigeRoll> SewinQueue { get; }
    void AddRoll();
  }
}
