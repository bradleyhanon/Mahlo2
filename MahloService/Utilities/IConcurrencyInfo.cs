using System.Reactive.Concurrency;
using System.Threading;

namespace MahloService.Utilities
{
  internal interface IConcurrencyInfo
  {
    SynchronizationContext SynchronizationContext { get; }
    IScheduler Scheduler { get; }
  }
}
