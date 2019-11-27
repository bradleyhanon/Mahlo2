using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;
using MahloService.Settings;
using MahloService.Utilities;
using Serilog;

namespace MahloService.Logic
{
  internal sealed class SewinQueue : ISewinQueue, IEqualityComparer<GreigeRoll>, INotifyPropertyChanged
  {
    private int maxRollsToKeep = 200;
    private readonly ILogger logger;
    private int nextRollId;

    private readonly IDbLocal dbLocal;
    private readonly IDbMfg dbMfg;
    private readonly IServiceSettings settings;

    private readonly Queue<string> messageQueue = new Queue<string>();
    private bool isRefreshBusy;

    private string priorFirstRoll = string.Empty;
    private string priorLastRoll = string.Empty;
    private int? priorQueueSize;
    private IDisposable timer;

    public SewinQueue(IScheduler scheduler, IDbLocal dbLocal, IDbMfg dbMfg, IServiceSettings settings, ILogger logger)
    {
      this.dbLocal = dbLocal;
      this.dbMfg = dbMfg;
      this.settings = settings;
      this.logger = logger;

      this.nextRollId = this.dbLocal.GetNextGreigeRollId();
      this.maxRollsToKeep = settings.MaxSewinQueueRolls;
      this.Rolls.AddRange(this.dbLocal.GetRecentGreigeRolls(this.maxRollsToKeep));

      var ignoredResultTask = this.RefreshIfChangedAsync();
      this.timer = Observable
        .Interval(RefreshInterval, scheduler)
        .Subscribe(_ => this.RefreshIfChangedAsync().NoWait());
    }

    public event Action QueueChanged;

    public event CancelEventHandler CanRemoveRollQuery;

    public event PropertyChangedEventHandler PropertyChanged;

    public static TimeSpan RefreshInterval => TimeSpan.FromSeconds(10);

    public bool IsChanged { get; set; }

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

        HashSet<GreigeRoll> seenRolls = new HashSet<GreigeRoll>(
          this.Rolls.Where(roll => roll.IsComplete));

        // Update or add from the new rolls
        int updatedCount = 0;
        int addedCount = 0;
        int lastIdUpdated = 0;
        foreach (var newRoll in newRolls)
        {
          var oldRoll = this.Rolls.FirstOrDefault(item => item.RollNo == newRoll.RollNo && !seenRolls.Contains(item) &&
            (!newRoll.IsCheckRoll || item.Id > lastIdUpdated));
          if (oldRoll != null)
          {
            // Update old rolls we already have
            newRoll.CopyTo(oldRoll);
            this.dbLocal.UpdateGreigeRoll(oldRoll);
            updatedCount++;
            seenRolls.Add(oldRoll);
            lastIdUpdated = oldRoll.Id;
            //Console.WriteLine($"Upd Roll={newRoll.RollNo}");
          }
          else
          {
            // Add new rolls
            newRoll.Id = this.nextRollId++;
            this.Rolls.Add(newRoll);
            this.dbLocal.AddGreigeRoll(newRoll);
            addedCount++;
            seenRolls.Add(newRoll);
            //Console.WriteLine($"Add Roll={newRoll.RollNo}");
          }
        }

        // Set rolls that have been removed from the queue as complete
        var rollsRemoved = this.Rolls.Except(seenRolls).ToList();
        this.dbLocal.SetGreigeRollsComplete(rollsRemoved);

        this.logger.Debug("SewinQueue refreshed: added={added}, updated={updated}, removed={removed}", addedCount, updatedCount, rollsRemoved.Count);
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
        if (await this.dbMfg.GetIsSewinQueueChangedAsync(this.priorQueueSize ?? 0, this.priorFirstRoll, this.priorLastRoll))
        {
          this.SetMessage("Reading queue");
          await this.RefreshAsync();
        }
        //else
        //{
        //  this.logger.Debug("SewinQueue unchanged: size={priorQueueSize}, first={priorFirstRoll}, last={priorLastRoll}.", this.priorQueueSize, this.priorFirstRoll, this.priorLastRoll);
        //}

        if (this.priorQueueSize.HasValue)
        {
          // Remove rolls that are no longer needed for display
          CancelEventArgs args = new CancelEventArgs();
          while (this.Rolls.Count > this.maxRollsToKeep + this.priorQueueSize.Value)
          {
            args.Cancel = false;
            this.CanRemoveRollQuery?.Invoke(this.Rolls[0], args);

            if (args.Cancel)
            {
              break;
            }

            this.Rolls.RemoveAt(0);
          }
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