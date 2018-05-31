using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Repository;
using MahloService.Utilities;
using MahloServiceTests.Mocks;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Xunit;

namespace MahloServiceTests
{
  public class SewinQueueTests : IEqualityComparer<CarpetRoll>, IDisposable
  {
    private CarpetRoll roll1 = new CarpetRoll() { RollNo = "100" };
    private CarpetRoll roll2 = new CarpetRoll() { RollNo = "200" };
    private CarpetRoll roll3 = new CarpetRoll() { RollNo = "300" };
    private CarpetRoll roll4 = new CarpetRoll() { RollNo = "400" };
    private CarpetRoll roll5 = new CarpetRoll() { RollNo = "500" };

    IDbLocal dbLocal = Substitute.For<IDbLocal>();
    IDbMfg dbMfg = Substitute.For<IDbMfg>();
    TestSchedulers schedulers = new TestSchedulers();

    SewinQueue target;

    public SewinQueueTests()
    {
    }

    public void Dispose()
    {
      this.target?.Dispose();
    }

    [Fact]
    public void GetIsSewinQueueChangedIsCalledPeriodically()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(false);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);
      for (int j = 1; j < 5; j++)
      {
        this.schedulers.WinFormsThread.AdvanceBy(target.RefreshInterval.Ticks - 1);
        dbMfg.Received(j).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
        this.schedulers.WinFormsThread.AdvanceBy(1);
        dbMfg.Received(j + 1).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
      }
    }

    [Fact]
    public void GetCoaterSewinQueueIsNotCalledWhenGetIsQueChangedReturnsFalse()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(false);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);
      dbMfg.Received(1).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
      dbMfg.DidNotReceive().GetCoaterSewinQueue();
    }

    [Fact]
    public void GetCoaterSewinQueueIsCalledWhenGetIsQueChangedReturnsTrue()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);
      dbMfg.Received(1).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
      dbMfg.Received(1).GetCoaterSewinQueue();
    }

    [Fact]
    public void GetIsSewinQueueChangedCalledWithResultsFromPriorGetCoaterSewinQueue()
    {
      var newRolls = new CarpetRoll[] { roll1, roll2, roll3 };
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);

      this.dbMfg.ClearReceivedCalls();
      this.schedulers.WinFormsThread.AdvanceBy(target.RefreshInterval.Ticks);
      this.dbMfg.Received(1).GetIsSewinQueueChanged(3, roll1.RollNo, roll3.RollNo);
    }

    [Fact]
    public void EmptyQueueIsPopulatedWithRecords()
    {
      var newRolls = new CarpetRoll[] { roll1, roll2, roll3 };
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);
      Assert.True(newRolls.SequenceEqual(this.target.Rolls));

      for (int j = 0; j < this.target.Rolls.Count; j++)
      {
        Assert.Equal(j + 1, this.target.Rolls[j].Id);
      }
    }

    [Fact]
    public void OldRollsAreUpdatedAndNewRollsAreAdded()
    {
      roll3.ProductImageURL = "Unchanged";
      var newRolls1 = new CarpetRoll[] { Clone(roll2), Clone(roll3), Clone(roll4) };
      roll3.ProductImageURL = "Now changed";
      var newRolls2 = new CarpetRoll[] { Clone(roll3), Clone(roll4), Clone(roll5), Clone(roll1) };
      var expected = new CarpetRoll[] { Clone(roll2), Clone(roll3), Clone(roll4), Clone(roll5), Clone(roll1) };

      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls1);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);
      this.dbMfg.Received(1).GetCoaterSewinQueue();

      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChanged(3, roll2.RollNo, roll4.RollNo).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls2);
      this.schedulers.WinFormsThread.AdvanceBy(target.RefreshInterval.Ticks);
      this.dbMfg.Received(1).GetCoaterSewinQueue();
      Assert.True(target.Rolls.SequenceEqual(expected, this));
      Assert.Equal("Now changed", target.Rolls.Single(item => item.RollNo == roll3.RollNo).ProductImageURL);
    }

    private CarpetRoll Clone(CarpetRoll roll)
    {
      CarpetRoll result = new CarpetRoll();
      roll.CopyTo(result);
      return result;
    }

    public bool Equals(CarpetRoll x, CarpetRoll y)
    {
      return x.RollNo == y.RollNo;
    }

    public int GetHashCode(CarpetRoll obj)
    {
      throw new NotImplementedException();
    }
  }
}
