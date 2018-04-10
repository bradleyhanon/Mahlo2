using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Utilities
{
  public interface ISchedulerProvider
  {
    IScheduler CurrentThread { get; }
    IScheduler Dispatcher { get; }
    IScheduler Immediate { get; }
    IScheduler NewThread { get; }
    IScheduler ThreadPool { get; }
    IScheduler Default { get; }
    //IScheduler TaskPool { get; } 
  }
}
