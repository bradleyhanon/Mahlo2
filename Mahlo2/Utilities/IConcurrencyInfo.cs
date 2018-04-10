using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mahlo.Utilities
{
  interface IConcurrencyInfo
  {
    SynchronizationContext SynchronizationContext { get; }
    ISchedulerProvider SchedulerProvider { get; }
  }
}
