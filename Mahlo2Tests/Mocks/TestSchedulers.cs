using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Utilities;
using Microsoft.Reactive.Testing;

namespace Mahlo2Tests.Mocks
{
  sealed class TestSchedulers : ISchedulerProvider
  {
    private readonly TestScheduler _currentThread = new TestScheduler();
    private readonly TestScheduler _dispatcher = new TestScheduler();
    private readonly TestScheduler _immediate = new TestScheduler();
    private readonly TestScheduler _newThread = new TestScheduler();
    private readonly TestScheduler _threadPool = new TestScheduler();
    private readonly TestScheduler _default = new TestScheduler();
    #region Implementation of ISchedulerService
    IScheduler ISchedulerProvider.CurrentThread => _currentThread;
    IScheduler ISchedulerProvider.Dispatcher => _dispatcher;
    IScheduler ISchedulerProvider.Immediate => _immediate;
    IScheduler ISchedulerProvider.NewThread => _newThread;
    IScheduler ISchedulerProvider.ThreadPool => _threadPool;
    IScheduler ISchedulerProvider.Default => _default;
    #endregion
    public TestScheduler CurrentThread => _currentThread;
    public TestScheduler Dispatcher => _dispatcher;
    public TestScheduler Immediate => _immediate;
    public TestScheduler NewThread => _newThread;
    public TestScheduler ThreadPool => _threadPool;
    public TestScheduler Default => _default;
  }
}
