using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mahlo.Utilities
{
  class ConcurrencyInfo : IConcurrencyInfo
  {
    public ConcurrencyInfo (SynchronizationContext synchronizationContext, ISchedulerProvider schedulerProvider)
    {
      this.SynchronizationContext = synchronizationContext;
      this.SchedulerProvider = schedulerProvider;
    }

    public SynchronizationContext SynchronizationContext { get; }

    public ISchedulerProvider SchedulerProvider { get; }
  }
}
