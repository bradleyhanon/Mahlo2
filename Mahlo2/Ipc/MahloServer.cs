using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace Mahlo.Ipc
{
  sealed class MahloServer : IMahloServer, IDisposable
  {
    private ILogger log;
    private ISewinQueue sewinQueue;
    private IMahloLogic mahloLogic;
    private IBowAndSkewLogic bowAndSkewLogic;
    private IPatternRepeatLogic patternRepeatLogic;
    private IDisposable timer;

    public MahloServer(
      ILogger logger,
      ISewinQueue sewinQueue,
      IMahloLogic mahloLogic,
      IBowAndSkewLogic bowAndSkewLogic,
      IPatternRepeatLogic patternRepeatLogic,
      ISchedulerProvider schedulerProvider)
    {
      this.log = logger;
      this.sewinQueue = sewinQueue;
      this.mahloLogic = mahloLogic;
      this.bowAndSkewLogic = bowAndSkewLogic;
      this.patternRepeatLogic = patternRepeatLogic;

      this.timer = Observable
        .Interval(TimeSpan.FromMilliseconds(1000), schedulerProvider.WinFormsThread)
        .Subscribe(_ =>
        {
          this.UpdateMahloLogic();
          this.UpdateBowAndSkewLogic();
          this.UpdatePatternRepeatLogic();
        });

      sewinQueue.QueueChanged.Subscribe(_ => UpdateSewinQueue());
    }

    private IHubConnectionContext<dynamic> Clients { get; set; } = GlobalHost.ConnectionManager.GetHubContext<MahloHub>().Clients;

    public void Dispose()
    {
      this.timer?.Dispose();
    }

    public void UpdateSewinQueue()
    {
      this.Clients.All.UpdateSewinQueue(this.sewinQueue.Rolls.ToArray());
    }

    public void UpdateMahloLogic()
    {
      if (this.mahloLogic.IsChanged)
      {
        log.Debug("UpdateMahloLogic()");
        this.mahloLogic.IsChanged = false;
        this.Clients.All.UpdateMahloLogic(this.mahloLogic);
      }
    }

    public void UpdateBowAndSkewLogic()
    {
      if (this.bowAndSkewLogic.IsChanged)
      {
        log.Debug("UpdateBowAndSkewLogic()");
        this.bowAndSkewLogic.IsChanged = false;
        this.Clients.All.UpdateBowAndSkewLogic(this.bowAndSkewLogic);
      }
    }

    public void UpdatePatternRepeatLogic()
    {
      if (this.patternRepeatLogic.IsChanged)
      {
        log.Debug("UpdatePatternRepeatLogic()");
        this.patternRepeatLogic.IsChanged = false;
        this.Clients.All.UpdatePatternRepeatLogic(this.patternRepeatLogic);
      }
    }

    public void RefreshAll(string connectionId)
    {
      var client = this.Clients.Client(connectionId);
      client.UpdateSewinQueue(this.sewinQueue.Rolls.ToArray());
      client.UpdateMahloLogic(this.mahloLogic);
      client.UpdateBowAndSkewLogic(this.bowAndSkewLogic);
      client.UpdatePatternRepeatLogic(this.patternRepeatLogic);
    }
  }
}
