using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MahloService.Utilities
{
  interface IConcurrencyInfo
  {
    SynchronizationContext SynchronizationContext { get; }
    ISchedulerProvider SchedulerProvider { get; }
  }
}
