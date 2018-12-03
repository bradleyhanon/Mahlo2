using System.Threading;

namespace MahloService.Utilities
{
  public sealed class SchedulerProvider : ISchedulerProvider
  {
    public SchedulerProvider(SynchronizationContext winFormsSynchronizationContext)
    {
      this.WinFormsThread = new SynchronizationContextScheduler(winFormsSynchronizationContext);
    }

    public IScheduler CurrentThread => Scheduler.CurrentThread;
    public IScheduler Dispatcher => DispatcherScheduler.Current;
    public IScheduler Immediate => Scheduler.Immediate;
    public IScheduler NewThread => NewThreadScheduler.Default;
    public IScheduler ThreadPool => Scheduler.Default;
    public IScheduler Default => Scheduler.Default;
    public IScheduler WinFormsThread { get; }
  }
}
