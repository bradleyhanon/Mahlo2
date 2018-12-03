using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;
using MahloService.Utilities;

namespace MahloService.Logic
{
  sealed class SapRollAssigner : ISapRollAssigner, IDisposable
  {
    public readonly TimeSpan TryInterval = TimeSpan.FromSeconds(2);
    private readonly IDbMfg dbMfg;
    private readonly IDbLocal dbLocal;
    private readonly IScheduler scheduler;
    private decimal priorSapRoll;
    private readonly IDisposable subscription;

    private CutRoll cutRoll;
    private bool busy;

    public SapRollAssigner(IDbMfg dbMfg, IDbLocal dbLocal, IScheduler scheduler)
    {
      this.dbMfg = dbMfg;
      this.dbLocal = dbLocal;
      this.scheduler = scheduler;
      this.InitializePriorSapRollAsync().NoWait();

      this.subscription = Observable
        .Interval(this.TryInterval, this.scheduler)
        .Where(_ => this.cutRoll != null)
        .Subscribe(_ => this.AssignSapRollAsync().NoWait());
    }

    private async Task AssignSapRollAsync()
    {
      if (!this.busy)
      {
        this.busy = true;
        decimal? sapRoll = await this.dbMfg.GetCutRollFromHostAsync();
        if (sapRoll != null && sapRoll.Value != this.priorSapRoll)
        {
          this.priorSapRoll = sapRoll.Value;
          this.cutRoll.SapRoll = sapRoll.Value.ToString();
          this.dbLocal.UpdateCutRoll(this.cutRoll);
          this.cutRoll = null;
        }

        this.busy = false;
      }
    }

    public void Dispose()
    {
      this.subscription.Dispose();
    }

    /// <summary>
    /// Make a best effort to assign a SAP roll number to the cut roll
    /// and update the database.
    /// </summary>
    /// <param name="cutRoll">The </param>
    /// <returns></returns>
    public void AssignSapRollTo(CutRoll cutRoll)
    {
      if (this.cutRoll != null)
      {
        // No SAP roll number was assigned
        // The next roll needs it, so we just have update without a SAP roll number
        this.dbLocal.UpdateCutRoll(this.cutRoll);
      }

      // Make the new roll available to be assigned a SAP rull number
      this.cutRoll = cutRoll;
    }

    private async Task InitializePriorSapRollAsync()
    {
      this.priorSapRoll = await this.dbMfg.GetCutRollFromHostAsync() ?? 0;
    }
  }
}
