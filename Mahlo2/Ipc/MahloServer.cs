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
        .Interval(TimeSpan.FromMilliseconds(250), schedulerProvider.WinFormsThread)
        .Subscribe(_ =>
        {
          this.UpdateMahloLogic(false);
          this.UpdateBowAndSkewLogic(false);
          this.UpdatePatternRepeatLogic(false);
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

    public void UpdateMahloLogic(bool always)
    {
      if (this.mahloLogic.IsChanged || always)
      {
        this.mahloLogic.IsChanged = false;
        this.Clients.All.UpdateMahloLogic(this.mahloLogic);
      }
    }

    public void UpdateBowAndSkewLogic(bool always)
    {
      if (this.bowAndSkewLogic.IsChanged || always)
      {
        this.bowAndSkewLogic.IsChanged = false;
        this.Clients.All.UpdateBowAndSkewLogic(this.bowAndSkewLogic);
      }
    }

    public void UpdatePatternRepeatLogic(bool always)
    {
      if (this.patternRepeatLogic.IsChanged || always)
      {
        this.patternRepeatLogic.IsChanged = false;
        this.Clients.All.UpdatePatternRepeatLogic(this.patternRepeatLogic);
      }
    }

    public void RefreshAll()
    {
      this.UpdateSewinQueue();
      this.UpdateMahloLogic(true);
      this.UpdateBowAndSkewLogic(true);
      this.UpdatePatternRepeatLogic(true);
    }
  }
}
