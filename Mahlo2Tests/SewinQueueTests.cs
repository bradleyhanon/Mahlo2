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
  public class SewinQueueTests : IEqualityComparer<GreigeRoll>
  {
    private GreigeRoll roll1 = new GreigeRoll() { RollNo = "100" };
    private GreigeRoll roll2 = new GreigeRoll() { RollNo = "200" };
    private GreigeRoll roll3 = new GreigeRoll() { RollNo = "300" };
    private GreigeRoll roll4 = new GreigeRoll() { RollNo = "400" };
    private GreigeRoll roll5 = new GreigeRoll() { RollNo = "500" };

    IDbLocal dbLocal = Substitute.For<IDbLocal>();
    IDbMfg dbMfg = Substitute.For<IDbMfg>();
    TestSchedulers schedulers = new TestSchedulers();

    SewinQueue target;

    public SewinQueueTests()
    {
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
      var newRolls = new GreigeRoll[] { roll1, roll2, roll3 };
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
      var newRolls = new GreigeRoll[] { roll1, roll2, roll3 };
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);
      Assert.True(newRolls.SequenceEqual(this.target.Rolls));

      for (int j = 0; j < this.target.Rolls.Count; j++)
      {
        Assert.Equal(j + 1, this.target.Rolls[j].RollId);
      }
    }

    [Fact]
    public void OldRollsAreUpdatedAndNewRollsAreAdded()
    {
      roll3.ProductImageURL = "Unchanged";
      var newRolls1 = new GreigeRoll[] { Clone(roll2), Clone(roll3), Clone(roll4) };
      roll3.ProductImageURL = "Now changed";
      var newRolls2 = new GreigeRoll[] { Clone(roll3), Clone(roll4), Clone(roll5), Clone(roll1) };
      var expected = new GreigeRoll[] { Clone(roll2), Clone(roll3), Clone(roll4), Clone(roll5), Clone(roll1) };

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

    [Fact]
    public void GetRollGetsTheRollRequested()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(new[] { roll1, roll2, roll3 });
      target = new SewinQueue(schedulers, dbLocal, dbMfg);

      var expected = roll2;
      var actual = this.target.GetRoll(2);
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetRollGetsTheNextRollIfTheFirstDoesntExist()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(new[] { roll1, roll3 });
      target = new SewinQueue(schedulers, dbLocal, dbMfg);

      var expected = roll1;
      var actual = this.target.GetRoll(0);
      Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetRollCreatesNewRollIfThereIsNoNextRoll()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(new[] { roll1, roll2, roll3 });
      target = new SewinQueue(schedulers, dbLocal, dbMfg);

      var roll = this.target.GetRoll(10);
      Assert.Equal(4, roll.RollId);
      Assert.Contains(roll, target.Rolls);
    }

    [Fact]
    public void ConstructorReadsRollsFromDatabaseAndSetsNextRollId()
    {
      GreigeRoll[] dbRolls = new[] { roll2, roll3, roll4 };
      roll2.RollId = 2;
      roll3.RollId = 3;
      roll4.RollId = 4;

      this.dbLocal.GetGreigeRolls().Returns(dbRolls);
      target = new SewinQueue(schedulers, dbLocal, dbMfg);
      Assert.True(dbRolls.SequenceEqual(target.Rolls));

      var roll = this.target.GetRoll(10);
      Assert.Equal(5, roll.RollId);
    }


    private GreigeRoll Clone(GreigeRoll roll)
    {
      GreigeRoll result = new GreigeRoll();
      roll.CopyTo(result);
      return result;
    }

    public bool Equals(GreigeRoll x, GreigeRoll y)
    {
      return x.RollNo == y.RollNo;
    }

    public int GetHashCode(GreigeRoll obj)
    {
      throw new NotImplementedException();
    }
  }
}
