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

    public void RefreshAll()
    {
      this.mahloServer.RefreshAll(this.Context.ConnectionId);
    }

    public void MoveToPriorRoll(string name)
    {
      this.syncContext.Post(_ => this.GetMeterLogicInstance(name).MoveToPriorRoll(), null);
    }

    public void MoveToNextRoll(string name)
    {
      this.syncContext.Post(_ => this.GetMeterLogicInstance(name).MoveToNextRoll(), null);
    }

    public void WaitForSeam(string name)
    {
      this.syncContext.Post(_ => this.GetMeterLogicInstance(name).WaitForSeam(), null);
    }

    private IMeterLogic GetMeterLogicInstance(string name)
    {
      switch (name)
      {
        case nameof(IMahloLogic):
          return Program.Container.GetInstance<IMahloLogic>();

        case nameof(IBowAndSkewLogic):
          return Program.Container.GetInstance<IBowAndSkewLogic>();

        case nameof(IPatternRepeatLogic):
          return Program.Container.GetInstance<IPatternRepeatLogic>();

        default:
          throw new InvalidOperationException($"MahloHub.GetMeterLogicInstance(\"{name}\")");
      }
    }
  }
}
