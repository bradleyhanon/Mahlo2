using System.ComponentModel;
using MahloService.Models;
using MahloService.Repository;

namespace MahloService.Simulation
{
  internal interface IDbMfgSim : IDbMfg
  {
    BindingList<GreigeRoll> SewinQueue { get; }
    void AddRoll();
    void RemoveRoll();
    void CutRollCompleted();
  }
}
