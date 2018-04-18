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

    public bool TryGetRoll(int rollId, out GreigeRoll roll)
    {
      roll = this.Rolls.FirstOrDefault(item => item.RollId == rollId);
      bool result = roll != null;
      if (!result)
      {
        roll = new GreigeRoll { RollId = rollId };
      }

      return result;
    }

    public bool RollIsLeader(int rollId)
    {
      var theRoll = this.Rolls.Single(item => item.RollId == rollId);
      if (!theRoll.IsCheckRoll)
      {
        return false;
      }

      var theIndex = this.Rolls.IndexOf(theRoll);

      string bk1 = "", bk2 = "";
      double width1 = 0, width2 = 0;

      int index = theIndex;
      while (--index >= 0)
      {
        var roll = this.Rolls[index];
        if (!roll.IsCheckRoll)
        {
          bk1 = roll.BackingCode;
          width1 = roll.RollWidth;
          break;
        }
      };

      index = theIndex;
      while (++index < this.Rolls.Count)
      {
        var roll = this.Rolls[index];
        if (!roll.IsCheckRoll)
        {
          bk2 = roll.BackingCode;
          width2 = roll.RollWidth;
          break;
        }
      };

      bool result = (bk1 == "XL" && bk2 != "XL") || width1 != width2;
      return result;
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

          // Skip newRolls that overlap with old rows
          //var rollsToAdd = newRolls.FindNewItems(this.Rolls, (a, b) => a.RollId == b.RollId);

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