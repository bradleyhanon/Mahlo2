using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Repository;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Xunit;

namespace MahloServiceTests.Logic
{
  public class SapRollAssignerTests
  {
    private readonly SapRollAssigner target;
    private readonly IDbLocal dbLocal;
    private readonly IDbMfg dbMfg;
    private readonly TestScheduler scheduler = new TestScheduler();

    public SapRollAssignerTests()
    {
      this.dbLocal = Substitute.For<IDbLocal>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbMfg.GetCutRollFromHostAsync().Returns(Task.FromResult<decimal?>(1));
      this.target = new SapRollAssigner(this.dbMfg, this.dbLocal, this.scheduler);
    }

    [Fact]
    public void NoRollAssignedUntilSapRollChanges()
    {
      CutRoll cutRoll = new CutRoll();

      this.target.AssignSapRollTo(cutRoll);
      Assert.Equal(string.Empty, cutRoll.SapRoll);
      this.scheduler.AdvanceBy(this.target.TryInterval.Ticks);
      Assert.Equal(string.Empty, cutRoll.SapRoll);
      this.dbLocal.DidNotReceiveWithAnyArgs().UpdateCutRoll(Arg.Any<CutRoll>());

      this.dbMfg.GetCutRollFromHostAsync().Returns(Task.FromResult<decimal?>(2));
      this.scheduler.AdvanceBy(this.target.TryInterval.Ticks);
      Assert.Equal("2", cutRoll.SapRoll);
      this.dbLocal.Received(1).UpdateCutRoll(cutRoll);
    }

    [Fact]
    public void FirstRollWrittenWithoutChangeIfSecondRollIsPresentedBeforeNewSapRollAvailable()
    {
      CutRoll cutRoll1 = new CutRoll();
      CutRoll cutRoll2 = new CutRoll();

      this.target.AssignSapRollTo(cutRoll1);
      this.scheduler.AdvanceBy(this.target.TryInterval.Ticks);
      this.target.AssignSapRollTo(cutRoll2);
      this.dbMfg.GetCutRollFromHostAsync().Returns(Task.FromResult<decimal?>(2));
      this.scheduler.AdvanceBy(this.target.TryInterval.Ticks);
      Assert.Equal(string.Empty, cutRoll1.SapRoll);
      Assert.Equal("2", cutRoll2.SapRoll);
      this.dbLocal.Received(1).UpdateCutRoll(cutRoll2);
      this.dbLocal.Received(1).UpdateCutRoll(cutRoll1);
    }
  }
}
