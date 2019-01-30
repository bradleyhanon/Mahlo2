using System.Collections.Generic;
using System.ComponentModel;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Settings;
using MahloServiceTests.Mocks;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Xunit;

namespace MahloServiceTests.Logic
{
  public class CutRollTests
  {
    private const double MinSeamSpacing = 4.0;
    private readonly PatternRepeatLogic target;
    private readonly IDbLocal dbLocal = Substitute.For<IDbLocal>();
    private readonly CutRollList cutRolls = new CutRollList();
    private readonly ISewinQueue sewinQueue = Substitute.For<ISewinQueue>();
    private readonly ISapRollAssigner sapRollAssigner = Substitute.For<ISapRollAssigner>();
    private readonly MockMeterSrc<PatternRepeatModel> patternRepeatSrc = new MockMeterSrc<PatternRepeatModel>();
    private readonly IServiceSettings serviceSettings = Substitute.For<IServiceSettings>();
    private readonly IUserAttentions<PatternRepeatModel> userAttentions = new UserAttentions<PatternRepeatModel>();
    private readonly ICriticalStops<PatternRepeatModel> criticalStops = new CriticalStops<PatternRepeatModel>();
    private readonly IProgramState programState = Substitute.For<IProgramState>();
    private readonly TestScheduler scheduler = new TestScheduler();

    public CutRollTests()
    {
      this.serviceSettings.MinSeamSpacing = MinSeamSpacing;
      var rolls = new BindingList<GreigeRoll>
      {
        new GreigeRoll { Id = 1, RollNo = "001" },
        new GreigeRoll { Id = 2, RollNo = "002" },
        new GreigeRoll { Id = 3, RollNo = "003" },
      };

      this.sewinQueue.Rolls.Returns(rolls);

      this.target = new PatternRepeatLogic(
        this.dbLocal, 
        this.cutRolls,
        this.sapRollAssigner,
        this.patternRepeatSrc,
        this.sewinQueue,
        this.serviceSettings,
        this.userAttentions,
        this.criticalStops,
        this.programState,
        this.scheduler);
    }

    [Fact]
    public void IfCutRollLengthLessThanMinSpacingItIsAssignedToNextGreigeRoll()
    {
      this.StartFirstGreigeRoll();
      this.patternRepeatSrc.FeetCounter += 1000;
      this.TriggerDoff();
      this.patternRepeatSrc.FeetCounter += MinSeamSpacing - 0.9999;
      this.TriggerSeam();

      Assert.Equal(2, this.target.CurrentCutRoll.GreigeRollId);
      Assert.Equal(1, this.cutRolls.Count);
      Assert.Equal(this.target.CurrentRoll.Id, this.cutRolls[0].GreigeRollId);
      Assert.Equal(this.target.CurrentRoll.Id, this.target.CurrentCutRoll.GreigeRollId);
    }

    [Fact]
    public void IfCutRollLengthGreaterOrEqualToMinSpacingItStaysWithItsAssignedGreigeRoll()
    {
      this.StartFirstGreigeRoll();
      this.patternRepeatSrc.FeetCounter += 1000;
      this.TriggerDoff();
      this.patternRepeatSrc.FeetCounter += MinSeamSpacing;
      this.TriggerSeam();

      Assert.Equal(1, this.target.CurrentCutRoll.GreigeRollId);
      Assert.Equal(0, this.cutRolls.Count);
      Assert.NotEqual(this.target.CurrentRoll.Id, this.target.CurrentCutRoll.GreigeRollId);
    }

    private void StartFirstGreigeRoll()
    {
      // Make the first greige roll into position
      while (this.target.CurrentRoll.Id == 0)
      {
        // Start a new Doff
        this.TriggerDoff();
        this.patternRepeatSrc.FeetCounter += MinSeamSpacing;
        this.TriggerSeam();
      }

      Assert.Equal(0, this.cutRolls.Count);
    }

    private void TriggerSeam()
    {
      this.patternRepeatSrc.IsSeamDetected = true;
      this.patternRepeatSrc.IsSeamDetected = false;
    }

    private void TriggerDoff()
    {
      this.patternRepeatSrc.IsDoffDetected = true;
      this.patternRepeatSrc.IsDoffDetected = false;
    }
  }
}
