using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace Mahlo.Ipc
{
  class MahloServer : IMahloServer
  {
    private ILogger log;
    private ISewinQueue sewinQueue;

    public MahloServer(ILogger logger, ISewinQueue sewinQueue)
    {
      this.log = logger;
      this.sewinQueue = sewinQueue;

      sewinQueue.QueueChanged.Subscribe(_ => UpdateSewinQueue());
    }

    public void UpdateSewinQueue()
    {
      this.Clients.All.UpdateSewinQueue(this.sewinQueue.Rolls.ToArray());
    }

    

    public void RefreshAll()
    {
      this.UpdateSewinQueue();
    }

    private IHubConnectionContext<dynamic> Clients { get; set; } = GlobalHost.ConnectionManager.GetHubContext<MahloHub>().Clients;
  }
}
