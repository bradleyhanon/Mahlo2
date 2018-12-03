using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task GetIsSewinQueueChangedIsCalledPeriodically()
    {
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(false);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      for (int j = 1; j < 5; j++)
      {
        this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks - 1);
        await this.dbMfg.Received(j).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
        this.scheduler.AdvanceBy(1);
        await this.dbMfg.Received(j + 1).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
      }
    }

    [Fact]
    public async Task GetCoaterSewinQueueIsNotCalledWhenGetIsQueChangedReturnsFalse()
    {
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(false);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      await this.dbMfg.Received(1).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
      await this.dbMfg.DidNotReceive().GetCoaterSewinQueueAsync();
    }

    [Fact]
    public async Task GetCoaterSewinQueueIsCalledWhenGetIsQueChangedReturnsTrue()
    {
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      await this.dbMfg.Received(1).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();
    }

    [Fact]
    public async Task GetIsSewinQueueChangedCalledWithResultsFromPriorGetCoaterSewinQueue()
    {
      var newRolls = new GreigeRoll[] { this.roll1, this.roll2, this.roll3 };
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);

      this.dbMfg.ClearReceivedCalls();
      this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks);
      await this.dbMfg.Received(1).GetIsSewinQueueChangedAsync(3, this.roll1.RollNo, this.roll3.RollNo);
    }

    [Fact]
    public void EmptyQueueIsPopulatedWithRecords()
    {
      var newRolls = new GreigeRoll[] { this.roll1, this.roll2, this.roll3 };
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbLocal.GetNextGreigeRollId().Returns(1);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      Assert.True(newRolls.SequenceEqual(this.target.Rolls));

      for (int j = 0; j < this.target.Rolls.Count; j++)
      {
        Assert.Equal(j + 1, this.target.Rolls[j].Id);
      }
    }

    [Fact]
    public async Task MatchingRollsUpdatedNewRollsAddedOthersRemoved()
    {
      this.roll3.ProductImageURL = "Unchanged";
      var newRolls1 = new GreigeRoll[] { Clone(this.roll2), Clone(this.roll3), Clone(this.roll4) };
      this.roll3.ProductImageURL = "Now changed";
      var newRolls2 = new GreigeRoll[] { Clone(this.roll4), Clone(this.roll3), Clone(this.roll5), Clone(this.roll1) };
      var expected = new GreigeRoll[] { Clone(this.roll3), Clone(this.roll4), Clone(this.roll5), Clone(this.roll1) };

      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();

      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChangedAsync(3, this.roll2.RollNo, this.roll4.RollNo).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls2);
      this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));
      Assert.Equal("Now changed", this.target.Rolls.Single(item => item.RollNo == this.roll3.RollNo).ProductImageURL);
    }

    [Fact]
    public async Task RollsStillInUseAreNotRemoved()
    {
      var newRolls1 = this.Clone(this.roll1, this.roll2, this.roll3).ToArray();
      var newRolls2 = this.Clone(this.roll3, this.roll4, this.roll5).ToArray();
      var expected = new GreigeRoll[] { this.roll2, this.roll3, this.roll4, this.roll5 };

      // Fill the queue with newRolls1
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.logger);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();

      this.target.CanRemoveRollQuery += (sender, e) => e.Cancel = sender == newRolls1[1];  // keep roll2

      // Update with newRolls2
      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChangedAsync(3, this.roll1.RollNo, this.roll3.RollNo).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls2);
      this.scheduler.AdvanceBy(this.target.RefreshInterval.Ticks);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();

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
