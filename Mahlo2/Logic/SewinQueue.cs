using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mahlo.Models;
using Mahlo.Repository;
using Mahlo.Utilities;

namespace Mahlo.Logic
{
  sealed class SewinQueue : ISewinQueue
  {
    private int firstRollId;
    private int nextRollId;

    private IDbLocal dbLocal;
    private IDbMfg dbMfg;

    private bool isRefreshBusy;

    private string priorFirstRoll = string.Empty;
    private string priorLastRoll = string.Empty;
    private int priorQueueSize;

    IDisposable timer;

    public SewinQueue(ISchedulerProvider schedulerProvider, IDbLocal dbLocal, IDbMfg dbMfg)
    {
      this.dbLocal = dbLocal;
      this.dbMfg = dbMfg;

      this.Rolls.AddRange(this.dbLocal.GetGreigeRolls());
      this.nextRollId = this.Rolls.LastOrDefault()?.RollId + 1 ?? 1;

      var ignoredResultTask = this.Refresh();
      this.timer = Observable
        .Interval(this.RefreshInterval, schedulerProvider.WinFormsThread)
        .Do(_ => Console.WriteLine("Timer Triggered!"))
        .Subscribe(async _ => await this.Refresh());
    }

    public TimeSpan RefreshInterval => TimeSpan.FromSeconds(10);

    public BindingList<GreigeRoll> Rolls { get; private set; } = new BindingList<GreigeRoll>();

    public void Dispose()
    {
      this.timer.Dispose();
    }

    public GreigeRoll GetRoll(int rollId)
    {
      GreigeRoll result;
      result = this.Rolls.FirstOrDefault(item => item.RollId >= rollId);
      if (result == null)
      {
        result = new GreigeRoll
        {
          RollId = this.nextRollId++
        };

        this.Rolls.Add(result);
      }

      return result;
    }

    public bool RollIsLeader(int rollId)
    {
      throw new NotImplementedException();
      //var TheRoll = this.Rolls.Single(item => item.RollId == rollId);

      //bool result = false;
      //try
      //{
      //  string sBk1 = "", sBk2 = "";
      //  double nWidth1 = 0, nWidth2 = 0;

      //  int index = rollId;
      //  while (--index >= this.firstRollId)
      //  {
      //    var roll = this.Rolls[index - this.firstRollId];
      //    if (roll.RollNo != GreigeRoll.CheckRollId)
      //    {
      //      sBk1 = roll.BackingCode;
      //      nWidth1 = roll.RollWidth;
      //      break;
      //    }
      //  };

      //  index = rollId;
      //  while (++index < this.nextRollId)
      //  {
      //    var roll = this.Rolls[index - this.firstRollId];
      //    if (roll.RollNo != GreigeRoll.CheckRollId)
      //    {
      //      sBk2 = roll.BackingCode;
      //      nWidth2 = roll.RollWidth;
      //      break;
      //    }
      //  };

      //  result = (sBk1 == "XL" && sBk2 != "XL") || nWidth1 != nWidth2;
      //}
      //catch
      //{
      //}

      //return result;
    }

    private async Task Refresh()
    {
      if (this.isRefreshBusy)
      {
        return;
      }

      this.isRefreshBusy = true;
      try
      {
        if (await this.dbMfg.GetIsSewinQueueChanged(this.priorQueueSize, this.priorFirstRoll, this.priorLastRoll))
        {
          var newRolls = (await this.dbMfg.GetCoaterSewinQueue()).ToArray();

          this.priorFirstRoll = newRolls.FirstOrDefault()?.RollNo ?? string.Empty;
          this.priorLastRoll = newRolls.LastOrDefault()?.RollNo ?? string.Empty;
          this.priorQueueSize = newRolls.Count();

          foreach (var newRoll in newRolls)
          {
            var oldRoll = this.Rolls.FirstOrDefault(item => item.RollNo == newRoll.RollNo);
            if (oldRoll != null)
            {
              newRoll.CopyTo(oldRoll);
              dbLocal.UpdateGreigeRoll(oldRoll);
            }
            else
            {
              newRoll.RollId = this.nextRollId++;
              this.Rolls.Add(newRoll);
              dbLocal.AddGreigeRoll(newRoll);
            }
          }

          //this.firstRollId = this.Rolls.Min(item => item.RollId);
        }
      }
      catch (Exception ex)
      {

      }

      this.isRefreshBusy = false;
    }
  }
}