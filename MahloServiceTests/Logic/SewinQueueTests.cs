using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Repository;
using MahloService.Settings;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Serilog;
using Xunit;

namespace MahloServiceTests
{
  public class SewinQueueTests : IEqualityComparer<GreigeRoll>, IDisposable
  {
    private readonly GreigeRoll roll1;
    private readonly GreigeRoll roll2;
    private readonly GreigeRoll roll3;
    private readonly GreigeRoll roll4;
    private readonly GreigeRoll roll5;

    private readonly IDbLocal dbLocal = Substitute.For<IDbLocal>();
    private readonly IDbMfg dbMfg = Substitute.For<IDbMfg>();
    private readonly IServiceSettings settings = Substitute.For<IServiceSettings>();
    private readonly TestScheduler scheduler = new TestScheduler();
    private readonly ILogger logger = Substitute.For<ILogger>();

    private SewinQueue target;

    public SewinQueueTests()
    {
      int nextId = 0;
      this.roll1 = new GreigeRoll() { RollNo = "100", Id = nextId++ };
      this.roll2 = new GreigeRoll() { RollNo = "200", Id = nextId++ };
      this.roll3 = new GreigeRoll() { RollNo = "300", Id = nextId++ };
      this.roll4 = new GreigeRoll() { RollNo = "400", Id = nextId++ };
      this.roll5 = new GreigeRoll() { RollNo = "500", Id = nextId++ };
    }

    public void Dispose()
    {
      this.target?.Dispose();
    }

    [Fact]
    public async Task GetIsSewinQueueChangedIsCalledPeriodically()
    {
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(false);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      for (int j = 1; j < 5; j++)
      {
        this.scheduler.AdvanceBy(SewinQueue.RefreshInterval.Ticks - 1);
        await this.dbMfg.Received(j).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
        this.scheduler.AdvanceBy(1);
        await this.dbMfg.Received(j + 1).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
      }
    }

    [Fact]
    public async Task GetCoaterSewinQueueIsNotCalledWhenGetIsQueChangedReturnsFalse()
    {
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(false);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      await this.dbMfg.Received(1).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
      await this.dbMfg.DidNotReceive().GetCoaterSewinQueueAsync();
    }

    [Fact]
    public async Task GetCoaterSewinQueueIsCalledWhenGetIsQueChangedReturnsTrue()
    {
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      await this.dbMfg.Received(1).GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();
    }

    [Fact]
    public async Task GetIsSewinQueueChangedCalledWithResultsFromPriorGetCoaterSewinQueue()
    {
      var newRolls = new GreigeRoll[] { this.roll1, this.roll2, this.roll3 };
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);

      this.dbMfg.ClearReceivedCalls();
      this.scheduler.AdvanceBy(SewinQueue.RefreshInterval.Ticks);
      await this.dbMfg.Received(1).GetIsSewinQueueChangedAsync(3, this.roll1.RollNo, this.roll3.RollNo);
    }

    [Fact]
    public void EmptyQueueIsPopulatedWithRecords()
    {
      var newRolls = new GreigeRoll[] { this.roll1, this.roll2, this.roll3 };
      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbLocal.GetNextGreigeRollId().Returns(1);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
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
      var newRolls1 = new GreigeRoll[] { this.Clone(this.roll2), this.Clone(this.roll3), this.Clone(this.roll4) };
      this.roll3.ProductImageURL = "Now changed";
      var newRolls2 = new GreigeRoll[] { this.Clone(this.roll4), this.Clone(this.roll3), this.Clone(this.roll5), this.Clone(this.roll1) };
      var expected = new GreigeRoll[] { this.Clone(this.roll3), this.Clone(this.roll4), this.Clone(this.roll5), this.Clone(this.roll1) };

      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();
      Assert.True(this.target.Rolls.SequenceEqual(newRolls1));

      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChangedAsync(3, this.roll2.RollNo, this.roll4.RollNo).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls2);
      this.scheduler.AdvanceBy(SewinQueue.RefreshInterval.Ticks);
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
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();

      this.target.CanRemoveRollQuery += RollInUseHandler;  // keep roll2

      // Update with newRolls2
      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChangedAsync(3, this.roll1.RollNo, this.roll3.RollNo).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls2);
      this.scheduler.AdvanceBy(SewinQueue.RefreshInterval.Ticks);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();

      // Verify the updated queue
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));

      this.target.CanRemoveRollQuery -= RollInUseHandler;
      return;

      void RollInUseHandler(object sender, CancelEventArgs e) => e.Cancel = sender == newRolls1[1];
    }


    [Fact]
    public async Task RollsInLimboAreRemovedWhenNoLongerReferenced()
    {
      // Roll2 should be removed
      var expected = new GreigeRoll[] { /*this.roll2,*/ this.roll3, this.roll4, this.roll5 };

      await this.RollsStillInUseAreNotRemoved();

      // Update with newRolls2
      this.dbMfg.ClearReceivedCalls();
      this.dbMfg.GetIsSewinQueueChangedAsync(this.target.Rolls.Count, this.target.Rolls[0].RollNo, this.target.Rolls.Last().RollNo).Returns(false);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(Enumerable.Empty<GreigeRoll>());
      this.scheduler.AdvanceBy(SewinQueue.RefreshInterval.Ticks);
      await this.dbMfg.DidNotReceive().GetCoaterSewinQueueAsync();

      // Verify the updated queue
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));
    }

    [Fact]
    public async Task SewinQueueCanHoldTwoCheckRolls()
    {
      var checkRoll1 = new GreigeRoll { RollNo = "CHKROL" };
      var checkRoll2 = new GreigeRoll { RollNo = "CHKROL" };
      var newRolls1 = this.Clone(this.roll1, checkRoll1, this.roll2, checkRoll2, this.roll3).ToArray();
      var expected = newRolls1;

      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));
    }

    [Fact]
    public async Task CheckRollsAreRemovedCorrectly()
    {
      var checkRoll1 = new GreigeRoll { RollNo = "CHKROL" };
      var checkRoll2 = new GreigeRoll { RollNo = "CHKROL" };
      var newRolls1 = (IEnumerable<GreigeRoll>)this.Clone(this.roll1, checkRoll1, this.roll2, checkRoll2, this.roll3).ToArray();
      var expected = newRolls1;

      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));

      do
      {
        // Update with newRolls1; remove one roll each time
        expected = newRolls1 = newRolls1.Skip(1);
        this.dbMfg.ClearReceivedCalls();
        this.dbMfg.GetIsSewinQueueChangedAsync(this.target.Rolls.Count, this.target.Rolls.First().RollNo, this.target.Rolls.Last().RollNo).Returns(true);
        this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
        this.scheduler.AdvanceBy(SewinQueue.RefreshInterval.Ticks);
        await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();

        // Verify the updated queue
        Assert.True(this.target.Rolls.SequenceEqual(expected, this));
      }
      while (newRolls1.Any());
    }

    [Fact]
    public async Task MultipleCheckRollsCycleThroughProperly()
    {
      int nextId = 1;
      var greigeRolls = new List<GreigeRoll>();
      for (int j = 0; j < 20; j++)
      {
        greigeRolls.Add(new GreigeRoll { RollNo = nextId++.ToString() });
        greigeRolls.Add(new GreigeRoll { RollNo = nextId++.ToString() });
        greigeRolls.Add(new GreigeRoll { RollNo = "CHKROL" });
      }

      var newRolls1 = greigeRolls.Take(7);
      var expected = newRolls1;

      this.dbMfg.GetIsSewinQueueChangedAsync(0, string.Empty, string.Empty).Returns(true);
      this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
      this.target = new SewinQueue(this.scheduler, this.dbLocal, this.dbMfg, this.settings, this.logger);
      await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();
      Assert.True(this.target.Rolls.SequenceEqual(expected, this));

      int skipCount = 0;
      do
      {
        // Update with newRolls1; shift one roll each time
        expected = newRolls1 = newRolls1.Skip(skipCount).Take(7);
        this.dbMfg.ClearReceivedCalls();
        this.dbMfg.GetIsSewinQueueChangedAsync(this.target.Rolls.Count, this.target.Rolls.First().RollNo, this.target.Rolls.Last().RollNo).Returns(true);
        this.dbMfg.GetCoaterSewinQueueAsync().Returns(newRolls1);
        this.scheduler.AdvanceBy(SewinQueue.RefreshInterval.Ticks);
        await this.dbMfg.Received(1).GetCoaterSewinQueueAsync();

        // Verify the updated queue
        Assert.True(this.target.Rolls.SequenceEqual(expected, this));
        skipCount++;
      }
      while (newRolls1.Any());
    }

    private GreigeRoll Clone(GreigeRoll roll)
    {
      GreigeRoll result = new GreigeRoll();
      roll.CopyTo(result);
      return result;
    }

    private IEnumerable<GreigeRoll> Clone(params GreigeRoll[] rolls)
    {
      foreach (var roll in rolls)
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
