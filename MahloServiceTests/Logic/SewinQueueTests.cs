using System;
using System.Collections.Generic;
using System.Linq;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Repository;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Serilog;
using Xunit;

namespace MahloServiceTests
{
  public class SewinQueueTests : IEqualityComparer<GreigeRoll>, IDisposable
  {
    private readonly GreigeRoll roll1 = new GreigeRoll() { RollNo = "100" };
    private readonly GreigeRoll roll2 = new GreigeRoll() { RollNo = "200" };
    private readonly GreigeRoll roll3 = new GreigeRoll() { RollNo = "300" };
    private readonly GreigeRoll roll4 = new GreigeRoll() { RollNo = "400" };
    private readonly GreigeRoll roll5 = new GreigeRoll() { RollNo = "500" };

    private readonly IDbLocal dbLocal = Substitute.For<IDbLocal>();
    private readonly IDbMfg dbMfg = Substitute.For<IDbMfg>();
    private readonly TestScheduler scheduler = new TestScheduler();
    private readonly ILogger logger = Substitute.For<ILogger>();

    private SewinQueue target;

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
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      for (int j = 1; j < 5; j++)
      {
        this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks - 1);
        this.dbMfg.Received(j).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
        this.scheduler.AdvanceBy(1);
        this.dbMfg.Received(j + 1).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
      }
    }

    [Fact]
    public void GetCoaterSewinQueueIsNotCalledWhenGetIsQueChangedReturnsFalse()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(false);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      this.dbMfg.Received(1).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
      this.dbMfg.DidNotReceive().GetCoaterSewinQueue();
    }

    [Fact]
    public void GetCoaterSewinQueueIsCalledWhenGetIsQueChangedReturnsTrue()
    {
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      this.dbMfg.Received(1).GetIsSewinQueueChanged(0, string.Empty, string.Empty);
      this.dbMfg.Received(1).GetCoaterSewinQueue();
    }

    [Fact]
    public void GetIsSewinQueueChangedCalledWithResultsFromPriorGetCoaterSewinQueue()
    {
      var newRolls = new GreigeRoll[] { this.roll1, this.roll2, this.roll3 };
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);

      this.dbMfg.ClearReceivedCalls();
      this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks);
      this.dbMfg.Received(1).GetIsSewinQueueChanged(3, this.roll1.RollNo, this.roll3.RollNo);
    }

    [Fact]
    public void EmptyQueueIsPopulatedWithRecords()
    {
      var newRolls = new GreigeRoll[] { this.roll1, this.roll2, this.roll3 };
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbLocal.GetNextGreigeRollId().Returns(1);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      Assert.True(newRolls.SequenceEqual(this.target.Rolls));

      for (int j = 0; j < this.target.Rolls.Count; j++)
      {
        Assert.Equal(j + 1, this.target.Rolls[j].Id);
      }
    }

    [Fact]
    public void MatchingRollsUpdatedNewRollsAddedOthersRemoved()
    {
      this.roll3.ProductImageURL = "Unchanged";
      var newRolls1 = new GreigeRoll[] { Clone(this.roll2), Clone(this.roll3), Clone(this.roll4) };
      this.roll3.ProductImageURL = "Now changed";
      var newRolls2 = new GreigeRoll[] { Clone(this.roll4), Clone(this.roll3), Clone(this.roll5), Clone(this.roll1) };
      var expected = new GreigeRoll[] { Clone(this.roll3), Clone(this.roll4), Clone(this.roll5), Clone(this.roll1) };

      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      this.dbMfg.Received(1).GetCoaterSewinQueue();

      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChanged(3, this.roll2.RollNo, this.roll4.RollNo).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls2);
      this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks);
      this.dbMfg.Received(1).GetCoaterSewinQueue();
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));
      Assert.Equal("Now changed", this.target.Rolls.Single(item => item.RollNo == this.roll3.RollNo).ProductImageURL);
    }

    [Fact]
    public void RollsStillInUseAreNotRemoved()
    {
      var newRolls1 = this.Clone(this.roll1, this.roll2, this.roll3).ToArray();
      var newRolls2 = this.Clone(this.roll3, this.roll4, this.roll5).ToArray();
      var expected = new GreigeRoll[] { this.roll2, this.roll3, this.roll4, this.roll5 };

      // Fill the queue with newRolls1
      this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      this.dbMfg.Received(1).GetCoaterSewinQueue();

      this.target.CanRemoveRollQuery += (sender, e) => e.Cancel = sender == newRolls1[1];  // keep roll2

      // Update with newRolls2
      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChanged(3, this.roll1.RollNo, this.roll3.RollNo).Returns(true);
      this.dbMfg.GetCoaterSewinQueue().Returns(newRolls2);
      this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks);
      this.dbMfg.Received(1).GetCoaterSewinQueue();

      // Verify the updated queue
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));
    }

    //[Fact]
    //public void RollsNotInUpdateAreRemoved()
    //{
    //  var newRolls1 = new GreigeRoll[] { Clone(roll2), Clone(roll3), Clone(roll4) };
    //  var newRolls2 = new GreigeRoll[] { Clone(roll3), Clone(roll4), Clone(roll5), Clone(roll1) };
    //  var expected = new GreigeRoll[] { Clone(roll2), Clone(roll3), Clone(roll4), Clone(roll5), Clone(roll1) };

    //  this.dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns(true);
    //  this.dbMfg.GetCoaterSewinQueue().Returns(newRolls1);
    //  target = new SewinQueue(schedulers, dbLocal, dbMfg);
    //  this.dbMfg.Received(1).GetCoaterSewinQueue();

    //  this.dbMfg.ClearReceivedCalls();
    //  this.dbMfg.GetIsSewinQueueChanged(3, roll2.RollNo, roll4.RollNo).Returns(true);
    //  this.dbMfg.GetCoaterSewinQueue().Returns(newRolls2);
    //  this.schedulers.WinFormsThread.AdvanceBy(target.RefreshInterval.Ticks);
    //  this.dbMfg.Received(1).GetCoaterSewinQueue();
    //  Assert.True(target.Rolls.SequenceEqual(expected, this));
    //  Assert.Equal("Now changed", target.Rolls.Single(item => item.RollNo == roll3.RollNo).ProductImageURL);
    //}

    private GreigeRoll Clone(GreigeRoll roll)
    {
      GreigeRoll result = new GreigeRoll();
      roll.CopyTo(result);
      return result;
    }

    private IEnumerable<GreigeRoll> Clone(params GreigeRoll[] rolls)
    {
      foreach(var roll in rolls)
      {
        GreigeRoll result = new GreigeRoll();
        roll.CopyTo(result);
        yield return result;
      }
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
