using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MahloService.Utilities
{
  class ConcurrencyInfo : IConcurrencyInfo
  {
    public ConcurrencyInfo (SynchronizationContext synchronizationContext, IScheduler scheduler)
    {
      this.SynchronizationContext = synchronizationContext;
      this.Scheduler = scheduler;
    }

    public SynchronizationContext SynchronizationContext { get; }

    public IScheduler Scheduler { get; }
  }
}
