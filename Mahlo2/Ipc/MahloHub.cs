using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Microsoft.AspNet.SignalR;

namespace Mahlo.Ipc
{
  public class MahloHub : Hub
  {
    CarpetProcessor mapper;

    public MahloHub()
    {
      this.mapper = Program.Container.GetInstance<CarpetProcessor>();
    }

    public IEnumerable<CarpetRoll> GetSewinQueue()
    {
      return this.mapper.SewinQueue.Rolls;
    }
  }
}
