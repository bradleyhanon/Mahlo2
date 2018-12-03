using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

namespace MahloService.Utilities
{
  internal static class TaskUtilities
  {
    private static readonly JoinableTaskFactory jtf = new JoinableTaskFactory(new JoinableTaskContext());

    public static void NoWait(this Task task, Action<Exception> errorAction = null)
    {
      var _ = task.ContinueWith(t => errorAction?.Invoke(t.Exception),
        CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Default);
    }

    public static async Task RunOnMainThreadAsync(Action action)
    {
      await jtf.SwitchToMainThreadAsync();
      action();
    }

    public static async Task RunOnMainThreadAsync(Func<Task> func)
    {
      await jtf.SwitchToMainThreadAsync();
      await func();
    }
  }
}
