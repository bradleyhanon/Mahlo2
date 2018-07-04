using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
    private readonly ILogger log;
    private ISewinQueue sewinQueue;
    private readonly CutRollList cutRolls;
    private IMahloLogic mahloLogic;
    private IBowAndSkewLogic bowAndSkewLogic;
    private IPatternRepeatLogic patternRepeatLogic;
    private readonly List<IDisposable> disposables;

    public MahloServer(
      ILogger logger,
      ISewinQueue sewinQueue,
      CutRollList cutRolls,
      IMahloLogic mahloLogic,
      IBowAndSkewLogic bowAndSkewLogic,
      IPatternRepeatLogic patternRepeatLogic,
      ISchedulerProvider schedulerProvider)
    {
      this.log = logger;
      this.sewinQueue = sewinQueue;
      this.cutRolls = cutRolls;
      this.mahloLogic = mahloLogic;
      this.bowAndSkewLogic = bowAndSkewLogic;
      this.patternRepeatLogic = patternRepeatLogic;

      this.disposables = new List<IDisposable>
      {
        Observable
          .Interval(TimeSpan.FromMilliseconds(1000), schedulerProvider.WinFormsThread)
          .Subscribe(_ =>
          {
            this.UpdateMeterLogic(nameof(IMahloLogic), this.mahloLogic);
            this.UpdateMeterLogic(nameof(IBowAndSkewLogic), this.bowAndSkewLogic);
            this.UpdateMeterLogic(nameof(IPatternRepeatLogic), this.patternRepeatLogic);
            this.UpdateCutRollList();
          }),

        Observable
          .FromEvent(
            h => this.sewinQueue.QueueChanged += h,
            h => this.sewinQueue.QueueChanged -= h)
          .Subscribe(_ => this.UpdateSewinQueue()),
      };
    }

    private IHubConnectionContext<dynamic> Clients { get; set; } = GlobalHost.ConnectionManager.GetHubContext<MahloHub>().Clients;

    public void Dispose()
    {
      this.disposables.ForEach(item => item.Dispose());
      this.disposables.Clear();
    }

    public void UpdateSewinQueue()
    {
      this.Clients.All.UpdateSewinQueue(this.sewinQueue.Rolls.ToArray());
    }

    public void UpdateCutRollList()
    {
      if (this.cutRolls.IsChanged)
      {
        this.cutRolls.IsChanged = false;
        this.Clients.All.UpdateCutRollList(this.cutRolls.ToArray());
      }
    }

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
      client.UpdateCutRollList(this.cutRolls.ToArray());
    }
  }
}
