using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo.Repository;
using Mahlo.Utilities;
using Mahlo2Tests.Mocks;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests
{
  public class SewinQueueTests
  {
    GreigeRoll[] rolls1 =
    {
      new GreigeRoll() { RollNo = "100" },
      new GreigeRoll() { RollNo = "200" },
      new GreigeRoll() { RollNo = "300" },
    };

    IDbLocal dbLocal = Substitute.For<IDbLocal>();
    IDbMfg dbMfg = Substitute.For<IDbMfg>();
    TestScheduler scheduler = new TestScheduler();
    TestSchedulers schedulers = new TestSchedulers();
    ConcurrencyInfo concurrencyInfo;

    SewinQueue sewinQueue;

    [Fact]
    public void Test()
    {
      concurrencyInfo = new ConcurrencyInfo(SynchronizationContext.Current, schedulers);
      dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns<bool>(false);
      sewinQueue = new SewinQueue(concurrencyInfo, dbLocal, dbMfg);
      dbMfg.Received().GetIsSewinQueueChanged(0, string.Empty, string.Empty);
    }
  }
}
