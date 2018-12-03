namespace MahloService.Utilities
{
  public interface ISchedulerProvider
  {
    IScheduler CurrentThread { get; }
    IScheduler Dispatcher { get; }
    IScheduler Immediate { get; }
    IScheduler NewThread { get; }
    IScheduler ThreadPool { get; }
    IScheduler Default { get; }
    IScheduler WinFormsThread { get; }
    //IScheduler TaskPool { get; } 
  }
}
