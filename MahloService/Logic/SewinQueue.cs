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

    //public bool RollIsLeader(int rollId)
    //{
    //  var theRoll = this.Rolls.Single(item => item.Id == rollId);
    //  if (!theRoll.IsCheckRoll)
    //  {
    //    return false;
    //  }

    //  var theIndex = this.Rolls.IndexOf(theRoll);

    //  string bk1 = "", bk2 = "";
    //  double width1 = 0, width2 = 0;

    //  int index = theIndex;
    //  while (--index >= 0)
    //  {
    //    var roll = this.Rolls[index];
    //    if (!roll.IsCheckRoll)
    //    {
    //      bk1 = roll.BackingCode;
    //      width1 = roll.RollWidth;
    //      break;
    //    }
    //  };

    //  index = theIndex;
    //  while (++index < this.Rolls.Count)
    //  {
    //    var roll = this.Rolls[index];
    //    if (!roll.IsCheckRoll)
    //    {
    //      bk2 = roll.BackingCode;
    //      width2 = roll.RollWidth;
    //      break;
    //    }
    //  };

    //  bool result = (bk1 == "XL" && bk2 != "XL") || width1 != width2;
    //  return result;
    //}

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

    //public void UpdateSewinQueue(IEnumerable<CarpetRoll> newRolls)
    //{
    //  var list = new List<ListItem>();

    //  // Update the overlapping rolls and add new rolls
    //  foreach (var newRoll in newRolls)
    //  {
    //    var info = this.Rolls
    //      .Select((oldRoll, index) => new { oldRoll, index })
    //      .FirstOrDefault(item => item.oldRoll.RollNo == newRoll.RollNo);

    //    if (info == null)
    //    {
    //      list.Add(new ListItem(this.Rolls.Count, newRoll));
    //      this.Rolls.Add(newRoll);
    //    }
    //    else
    //    {
    //      list.Add(new ListItem(info.index, info.oldRoll));
    //      newRoll.CopyTo(info.oldRoll);
    //    }
    //  }

    //  // Rearrange the rolls into the same order as the new sewin queue items
    //  int offset = this.Rolls.Count - list.Count;
    //  for (int j = list.Count - 1; j >= 0; j--)
    //  {
    //    int srcNdx = list[j].Index;
    //    int dstNdx = j + offset;
    //    if (list[j].Index != dstNdx)
    //    {
    //      // The index of the items after srcNdx (the item to be moved)
    //      // must be decreased by one
    //      for (int k = 0; k < j; k++)
    //      {
    //        if (list[k].Index > srcNdx)
    //        {
    //          list[k].Index--;
    //        }
    //      }

    //      // Move the roll to its new position
    //      var roll = this.Rolls[srcNdx];
    //      this.Rolls.Insert(dstNdx + 1, roll);
    //      this.Rolls.RemoveAt(srcNdx);
    //    }
    //  }

    //  this.Changed?.Invoke(this, EventArgs.Empty);
    //}

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

          //this.firstRollId = this.Rolls.Min(item => item.RollId);
        }

        this.Message = "Queue sleeping";
      }
      catch (Exception ex)
      {
        this.Message = $"Error: {ex.Message}";
      }

      this.isRefreshBusy = false;
    }
    //private class ListItem
    //{
    //  public int Index;
    //  public readonly CarpetRoll Roll;

    //  public ListItem(int index, CarpetRoll roll)
    //  {
    //    this.Index = index;
    //    this.Roll = roll;
    //  }
    //}
  }
}