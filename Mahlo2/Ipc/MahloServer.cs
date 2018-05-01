using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace Mahlo.Ipc
{
  class MahloServer : IMahloServer
  {
    private ILogger log;

    public MahloServer(ILogger logger)
    {
      this.log = logger;
    }

    public void UpdateSewinQueue(IEnumerable<CarpetRoll> rolls)
    {
      this.Clients.All.UpdateSewinQueue(rolls);
    }

    private IHubConnectionContext<dynamic> Clients { get; set; } = GlobalHost.ConnectionManager.GetHubContext<MahloHub>().Clients;
  }
}
