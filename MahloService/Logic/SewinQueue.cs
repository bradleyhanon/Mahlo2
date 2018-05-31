﻿using System;
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
  sealed class SewinQueue : ISewinQueue
  {
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

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action QueueChanged;

    public TimeSpan RefreshInterval => TimeSpan.FromSeconds(10);

    public BindingList<CarpetRoll> Rolls { get; private set; } = new BindingList<CarpetRoll>();
    public string Message { get; private set; }

    public void Dispose()
    {
      this.timer.Dispose();
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
  }
}