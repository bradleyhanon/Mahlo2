using System.Reactive.Concurrency;
using System.Threading;

namespace MahloService.Utilities
{
  internal class ConcurrencyInfo : IConcurrencyInfo
  {
    public ConcurrencyInfo(SynchronizationContext synchronizationContext, IScheduler scheduler)
    {
      this.SynchronizationContext = synchronizationContext;
      this.Scheduler = scheduler;
    }

    public SynchronizationContext SynchronizationContext { get; }

    public IScheduler Scheduler { get; }
  }
}
