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
    IMahloServer mahloServer;

    public MahloHub()
    {
      this.mahloServer = Program.Container.GetInstance<MahloServer>();
    }

    public override Task OnConnected()
    {
      this.mahloServer.RefreshAll();
      return base.OnConnected();
    }
  }
}
