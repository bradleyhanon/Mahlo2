using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;
using MahloService.Utilities;

namespace MahloService.Logic
{
  sealed class SewinQueue : ISewinQueue
  {
    private Subject<object> queueChanged = new Subject<object>();
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

      this.Rolls.AddRange(this.dbLocal.GetCarpetRolls());
      this.nextRollId = this.Rolls.LastOrDefault()?.Id + 1 ?? 1;

      var ignoredResultTask = this.RefreshIfChanged();
      this.timer = Observable
        .Interval(this.RefreshInterval, schedulerProvider.WinFormsThread)
        .Subscribe(async _ => await this.RefreshIfChanged());
    }

    public TimeSpan RefreshInterval => TimeSpan.FromSeconds(10);

    public BindingList<CarpetRoll> Rolls { get; private set; } = new BindingList<CarpetRoll>();

    public IObservable<object> QueueChanged => this.queueChanged;

    public void Dispose()
    {
      this.timer.Dispose();
    }

    public CarpetRoll GetRoll(int rollId)
    {
      CarpetRoll result;
      result = this.Rolls.FirstOrDefault(item => item.Id >= rollId);
      if (result == null)
      {
        result = new CarpetRoll
        {
          Id = this.nextRollId++
        };

        this.Rolls.Add(result);
      }

      return result;
    }

    public bool TryGetRoll(int rollId, out CarpetRoll roll)
    {
      roll = this.Rolls.FirstOrDefault(item => item.Id == rollId);
      bool result = roll != null;
      if (!result)
      {
        roll = new CarpetRoll { Id = rollId };
      }

      return result;
    }

    public bool RollIsLeader(int rollId)
    {
      var theRoll = this.Rolls.Single(item => item.Id == rollId);
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

    public async Task Refresh()
    {
      try
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
            dbLocal.UpdateCarpetRoll(oldRoll);
          }
          else
          {
            newRoll.Id = this.nextRollId++;
            this.Rolls.Add(newRoll);
            dbLocal.AddCarpetRoll(newRoll);
          }
        }
      }
      catch
      {

      }

      this.queueChanged.OnNext(this);
    }

    private async Task RefreshIfChanged()
    {
      if (this.isRefreshBusy)
      {
        return;
      }

      this.isRefreshBusy = true;
      try
      {
        await this.dbMfg.GetCutRollFromHost();
        if (await this.dbMfg.GetIsSewinQueueChanged(this.priorQueueSize, this.priorFirstRoll, this.priorLastRoll))
        {
          await Refresh();

          //this.firstRollId = this.Rolls.Min(item => item.RollId);
        }
      }
      catch
      {

      }

      this.isRefreshBusy = false;
    }
  }
}