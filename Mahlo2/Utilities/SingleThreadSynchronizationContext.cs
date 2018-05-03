using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading
{
  /// <summary>Provides a SynchronizationContext that's single-threaded.</summary>
  public sealed class SingleThreadSynchronizationContext : SynchronizationContext
  {
    /// <summary>The queue of work items.</summary>
    private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> m_queue =
        new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

    /// <summary>The processing thread.</summary>
    private readonly Thread m_thread = Thread.CurrentThread;

    /// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
    /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
    /// <param name="state">The object passed to the delegate.</param>
    public override void Post(SendOrPostCallback d, object state)
    {
      if (d == null)
      {
        throw new ArgumentNullException("d");
      }

      if (!this.m_queue.IsAddingCompleted)
      {
        m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
      }
    }

    /// <summary>Not supported.</summary>
    public override void Send(SendOrPostCallback d, object state)
    {
      throw new NotSupportedException("Synchronously sending is not supported.");
    }

    /// <summary>Runs an loop to process all queued work items.</summary>
    public void RunOnCurrentThread()
    {
      foreach (var workItem in m_queue.GetConsumingEnumerable())
      {
        workItem.Key(workItem.Value);
      }
    }

    /// <summary>Notifies the context that no more work will arrive.</summary>
    public void Complete() => m_queue.CompleteAdding();

    public override SynchronizationContext CreateCopy() => this;
  }
}
