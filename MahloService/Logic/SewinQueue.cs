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
using PropertyChanged;

namespace MahloService.Logic
{
  [AddINotifyPropertyChangedInterface]
  sealed class SewinQueue : ISewinQueue, IEqualityComparer<GreigeRoll>
  {
    private int nextRollId;

    private readonly IDbLocal dbLocal;
    private readonly IDbMfg dbMfg;

    private bool isRefreshBusy;

    private string priorFirstRoll = string.Empty;
    private string priorLastRoll = string.Empty;
    private int priorQueueSize;

    IDisposable timer;

    public SewinQueue(IScheduler scheduler, IDbLocal dbLocal, IDbMfg dbMfg)
    {
      this.dbLocal = dbLocal;
      this.dbMfg = dbMfg;

      this.nextRollId = this.dbLocal.GetNextGreigeRollId();
      this.Rolls.AddRange(this.dbLocal.GetIncompleteGreigeRolls());

      var ignoredResultTask = this.RefreshIfChanged();
      this.timer = Observable
        .Interval(this.RefreshInterval, scheduler)
        .Subscribe(async _ => await this.RefreshIfChanged());
    }

    public event Action QueueChanged;

    public bool IsChanged { get; set; }

    public TimeSpan RefreshInterval => TimeSpan.FromSeconds(10);

    public BindingList<GreigeRoll> Rolls { get; private set; } = new BindingList<GreigeRoll>();

    public string Message { get; private set; }

    public void Dispose()
    {
      this.timer.Dispose();
      this.timer = null;
    }

    public void MoveRoll(int rollIndex, int direction)
    {
      int newIndex = rollIndex + direction;
      if (rollIndex != newIndex && rollIndex >= 0 && rollIndex < this.Rolls.Count && newIndex >= 0 && newIndex < this.Rolls.Count)
      {
        var roll1 = this.Rolls[rollIndex];
        var roll2 = this.Rolls[newIndex];
        roll1.SwapWith(roll2);

        this.QueueChanged?.Invoke();
      }
    }

    public async Task Refresh()
    {
      try
      {
        var newRolls = (await this.dbMfg.GetCoaterSewinQueue()).ToArray();

        this.priorFirstRoll = newRolls.FirstOrDefault()?.RollNo ?? string.Empty;
        this.priorLastRoll = newRolls.LastOrDefault()?.RollNo ?? string.Empty;
        this.priorQueueSize = newRolls.Count();

        // Remove completed rolls
        var rollsToRemove = this.Rolls.Except(newRolls, this).ToArray();
        this.dbLocal.SetGreigeRollsComplete(rollsToRemove);
        rollsToRemove.ForEach(roll => this.Rolls.Remove(roll));

        foreach (var newRoll in newRolls)
        {
          var oldRoll = this.Rolls.FirstOrDefault(item => item.RollNo == newRoll.RollNo);
          if (oldRoll != null)
          {
            // Update old rolls we already have
            newRoll.CopyTo(oldRoll);
            this.dbLocal.UpdateGreigeRoll(oldRoll);
            //Console.WriteLine($"Upd Roll={newRoll.RollNo}");
          }
          else
          {
            // Add new rolls
            newRoll.Id = this.nextRollId++;
            this.Rolls.Add(newRoll);
            this.dbLocal.AddGreigeRoll(newRoll);
            //Console.WriteLine($"Add Roll={newRoll.RollNo}");
          }
        }
      }
      catch
      {

      }

      this.QueueChanged?.Invoke();
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
        this.Message = "Checking queue";
        await this.dbMfg.GetCutRollFromHost();
        if (await this.dbMfg.GetIsSewinQueueChanged(this.priorQueueSize, this.priorFirstRoll, this.priorLastRoll))
        {
          this.Message = "Reading queue";
          await Refresh();
        }

        this.Message = "Queue sleeping";
      }
      catch (Exception ex)
      {
        this.Message = $"Error: {ex.Message}";
      }

      this.isRefreshBusy = false;
    }

    // For IEqualityComparer<GreigeRoll>
    public bool Equals(GreigeRoll x, GreigeRoll y)
    {
      return x.RollNo == y.RollNo;
    }

    public int GetHashCode(GreigeRoll obj)
    {
      return obj.RollNo.GetHashCode();
    }
  }
}