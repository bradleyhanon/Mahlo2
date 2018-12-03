using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.Threading
{
  /// <summary>Provides a SynchronizationContext that's single-threaded.</summary>
  public sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
  {
    /// <summary>The queue of work items.</summary>
    private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> m_queue =
        new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

    public void Dispose()
    {
      this.m_queue.Dispose();
    }

    /// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
    /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
    /// <param name="state">The object passed to the delegate.</param>
    public override void Post(SendOrPostCallback d, object state)
    {
      if (d == null)
      {
        throw new ArgumentNullException(nameof(d));
      }

      if (!this.m_queue.IsAddingCompleted)
      {
        this.m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
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
      foreach (var workItem in this.m_queue.GetConsumingEnumerable())
      {
        workItem.Key(workItem.Value);
      }
    }

    /// <summary>Notifies the context that no more work will arrive.</summary>
    public void Complete() => this.m_queue.CompleteAdding();

    public override SynchronizationContext CreateCopy() => this;
  }
}
