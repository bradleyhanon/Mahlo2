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
using Serilog;

namespace MahloService.Logic
{
  sealed class SewinQueue : ISewinQueue, IEqualityComparer<GreigeRoll>, INotifyPropertyChanged
  {
    private readonly ILogger logger;
    private int nextRollId;

    private readonly IDbLocal dbLocal;
    private readonly IDbMfg dbMfg;

    private readonly Queue<string> messageQueue = new Queue<string>();
    private bool isRefreshBusy;

    private string priorFirstRoll = string.Empty;
    private string priorLastRoll = string.Empty;
    private int priorQueueSize;

    IDisposable timer;

    public SewinQueue(IScheduler scheduler, IDbLocal dbLocal, IDbMfg dbMfg, ILogger logger)
    {
      this.dbLocal = dbLocal;
      this.dbMfg = dbMfg;
      this.logger = logger;

      this.nextRollId = this.dbLocal.GetNextGreigeRollId();
      this.Rolls.AddRange(this.dbLocal.GetIncompleteGreigeRolls());

      var ignoredResultTask = this.RefreshIfChangedAsync();
      this.timer = Observable
        .Interval(this.RefreshInterval, scheduler)
        .Subscribe(_ => this.RefreshIfChangedAsync().NoWait());
    }

    public event Action QueueChanged;

    public event CancelEventHandler CanRemoveRollQuery;

    public event PropertyChangedEventHandler PropertyChanged;

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

    public async Task RefreshAsync()
    {
      try
      {
        var newRolls = (await this.dbMfg.GetCoaterSewinQueueAsync()).ToArray();

        this.priorFirstRoll = newRolls.FirstOrDefault()?.RollNo ?? string.Empty;
        this.priorLastRoll = newRolls.LastOrDefault()?.RollNo ?? string.Empty;
        this.priorQueueSize = newRolls.Count();

        // Remove completed rolls
        var rollsToRemove = this.Rolls.Except(newRolls, this).ToArray();
        var rollsRemoved = new List<GreigeRoll>();
        foreach (var roll in rollsToRemove)
        {
          CancelEventArgs args = new CancelEventArgs();
          this.CanRemoveRollQuery?.Invoke(roll, args);
          roll.IsInLimbo = args.Cancel;
          if (!args.Cancel)
          {
            rollsRemoved.Add(roll);
            this.Rolls.Remove(roll);
          }
        }

        this.dbLocal.SetGreigeRollsComplete(rollsRemoved);

        // Update or add from the new rolls
        int updatedCount = 0;
        int addedCount = 0;
        foreach (var newRoll in newRolls)
        {
          var oldRoll = this.Rolls.FirstOrDefault(item => item.RollNo == newRoll.RollNo);
          if (oldRoll != null)
          {
            // Update old rolls we already have
            newRoll.CopyTo(oldRoll);
            this.dbLocal.UpdateGreigeRoll(oldRoll);
            updatedCount++;
            //Console.WriteLine($"Upd Roll={newRoll.RollNo}");
          }
          else
          {
            // Add new rolls
            newRoll.Id = this.nextRollId++;
            this.Rolls.Add(newRoll);
            this.dbLocal.AddGreigeRoll(newRoll);
            addedCount++;
            //Console.WriteLine($"Add Roll={newRoll.RollNo}");
          }
        }

        this.logger.Debug("SewinQueue refreshed: added={added}, updated={updated}, removed={removed}", addedCount, updatedCount, rollsToRemove.Length);
      }
      catch (Exception ex)
      {
        this.logger.Debug("SewinQueue refresh failed: {exception}", ex.Message);
      }

      this.QueueChanged?.Invoke();
    }

    private async Task RefreshIfChangedAsync()
    {
      if (this.isRefreshBusy)
      {
        this.logger.Debug("Sewin queue refresh is busy.");
        return;
      }

      this.isRefreshBusy = true;
      try
      {
        this.SetMessage("Checking queue");
        if (await this.dbMfg.GetIsSewinQueueChangedAsync(this.priorQueueSize, this.priorFirstRoll, this.priorLastRoll))
        {
          this.SetMessage("Reading queue");
          await RefreshAsync();
        }
        else
        {
          this.logger.Debug("SewinQueue unchanged: size={priorQueueSize}, first={priorFirstRoll}, last={priorLastRoll}.", this.priorQueueSize, this.priorFirstRoll, this.priorLastRoll);
        }

        this.SetMessage("Queue sleeping");
      }
      catch (Exception ex)
      {
        this.SetMessage($"Error: {ex.Message}");
      }

      this.isRefreshBusy = false;
    }

    /// <summary>
    /// Set the message to displayed on the user's screen.
    /// </summary>
    /// <param name="message">The message to display.</param>
    private void SetMessage(string message)
    {
      // Delay setting the message until the prior message has been sent
      // This allows the user to read the first message before the next overwrites it.
      if (this.IsChanged)
      {
        this.messageQueue.Enqueue(message);
      }
      else
      {
        this.Message = message;
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      if (!this.IsChanged && propertyName == nameof(this.IsChanged) && this.messageQueue.Count > 0)
      {
        this.Message = this.messageQueue.Dequeue();
      }
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