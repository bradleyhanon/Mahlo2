using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Threading
{
  /// <summary>Provides a pump that supports running asynchronous methods on the current thread.</summary>
  public static class AsyncPump
  {
    /// <summary>Runs the specified asynchronous function.</summary>
    /// <param name="func">The asynchronous function to execute.</param>
    public static void Run(Func<Task> func)
    {
      if (func == null)
      {
        throw new ArgumentNullException("func");
      }

      var prevCtx = SynchronizationContext.Current;
      try
      {
        // Establish the new context
        var syncCtx = new SingleThreadSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext(syncCtx);

        // Invoke the function and alert the context to when it completes
        var t = func();
        if (t == null)
        {
          throw new InvalidOperationException("No task provided.");
        }

        t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

        // Pump continuations and propagate any exceptions
        syncCtx.RunOnCurrentThread();
        t.GetAwaiter().GetResult();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(prevCtx);
      }
    }
  }
}