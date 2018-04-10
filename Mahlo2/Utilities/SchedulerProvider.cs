using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.PlatformServices;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Utilities
{
  public sealed class SchedulerProvider : ISchedulerProvider
  {
    public IScheduler CurrentThread => Scheduler.CurrentThread;
    public IScheduler Dispatcher => DispatcherScheduler.Current;
    public IScheduler Immediate => Scheduler.Immediate;
    public IScheduler NewThread => NewThreadScheduler.Default;
    public IScheduler ThreadPool => Scheduler.Default;
    public IScheduler Default => Scheduler.Default;
    //public IScheduler TaskPool { get { return Scheduler.TaskPool; } } 
  }
}
