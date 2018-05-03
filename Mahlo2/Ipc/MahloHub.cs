using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Microsoft.AspNet.SignalR;

namespace Mahlo.Ipc
{
  public class MahloHub : Hub
  {
    IMahloServer mahloServer;
    SynchronizationContext syncContext;

    public MahloHub()
    {
      this.mahloServer = Program.Container.GetInstance<MahloServer>();
      this.syncContext = Program.Container.GetInstance<SynchronizationContext>();
    }

    public override Task OnConnected()
    {
      this.syncContext.Post(_ => this.mahloServer.RefreshAll(), null);
      return base.OnConnected();
    }
  }
}
