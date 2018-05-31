using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace MahloService.Ipc
{
  sealed class MahloServer : IMahloServer, IDisposable
  {
    private ILogger log;
    private ISewinQueue sewinQueue;
    private IMahloLogic mahloLogic;
    private IBowAndSkewLogic bowAndSkewLogic;
    private IPatternRepeatLogic patternRepeatLogic;
    private IDisposable timer;
    private IDisposable queueChangedSubscription;

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
          //this.UpdateMahloLogic();
          //this.UpdateBowAndSkewLogic();
          //this.UpdatePatternRepeatLogic();
          this.UpdateMeterLogic(nameof(IMahloLogic), this.mahloLogic);
          this.UpdateMeterLogic(nameof(IBowAndSkewLogic), this.bowAndSkewLogic);
          this.UpdateMeterLogic(nameof(IPatternRepeatLogic), this.patternRepeatLogic);
        });

      this.queueChangedSubscription = Observable
        .FromEvent(
          h => this.sewinQueue.QueueChanged += h,
          h => this.sewinQueue.QueueChanged -= h)
        .Subscribe(_ => this.UpdateSewinQueue());
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

    //public void UpdateMahloLogic()
    //{
    //  if (this.mahloLogic.IsChanged)
    //  {
    //    log.Debug("UpdateMahloLogic()");
    //    this.mahloLogic.IsChanged = false;
    //    this.Clients.All.UpdateMahloLogic(this.mahloLogic);
    //  }
    //}

    //public void UpdateBowAndSkewLogic()
    //{
    //  if (this.bowAndSkewLogic.IsChanged)
    //  {
    //    log.Debug("UpdateBowAndSkewLogic()");
    //    this.bowAndSkewLogic.IsChanged = false;
    //    this.Clients.All.UpdateBowAndSkewLogic(this.bowAndSkewLogic);
    //  }
    //}

    //public void UpdatePatternRepeatLogic()
    //{
    //  if (this.patternRepeatLogic.IsChanged)
    //  {
    //    log.Debug("UpdatePatternRepeatLogic()");
    //    this.patternRepeatLogic.IsChanged = false;
    //    this.Clients.All.UpdatePatternRepeatLogic(this.patternRepeatLogic);
    //  }
    //}

    public void UpdateMeterLogic<Model>(string interfaceName, IMeterLogic<Model> meterLogic)
    {
      if (meterLogic.IsChanged)
      {
        string name = typeof(Model).Name;
        //log.Debug($"UpdateMeterLogic<{interfaceName}>");
        meterLogic.IsChanged = false;
        this.Clients.All.UpdateMeterLogic(interfaceName, meterLogic);
      }
    }

    public void RefreshAll(string connectionId)
    {
      var client = this.Clients.Client(connectionId);
      client.UpdateSewinQueue(this.sewinQueue.Rolls.ToArray());
      client.UpdateMeterLogic(nameof(IMahloLogic), this.mahloLogic);
      client.UpdateMeterLogic(nameof(IBowAndSkewLogic), this.bowAndSkewLogic);
      client.UpdateMeterLogic(nameof(IPatternRepeatLogic), this.patternRepeatLogic);
    }
  }
}
