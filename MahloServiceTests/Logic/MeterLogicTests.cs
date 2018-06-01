using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloServiceTests.Mocks;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;
using System.Reactive.Linq;

namespace MahloServiceTests.Logic
{
  public sealed class MeterLogicTests : IDisposable
  {
    private MockMeterSrc<MahloRoll> srcData;
    private ISewinQueue sewinQueue;
    private IDbMfg dbMfg;
    private IDbLocal dbLocal;
    private IServiceSettings appInfo;
    private IUserAttentions<MahloRoll> userAttentions;
    private ICriticalStops<MahloRoll> criticalStops;
    private TestSchedulers testSchedulers = new TestSchedulers();
    private dynamic programState;

    private MeterLogic<MahloRoll> target;
    private List<CarpetRoll> carpetRolls;

    public MeterLogicTests()
    {
      const int roll1Length = 100;
      const int roll2Length = 200;
      const int roll3Length = 300;
      const int roll4Length = 400;
      const int roll5Length = 400;
      const int roll6Length = 400;
      const int roll7Length = 400;

      this.srcData = new MockMeterSrc<MahloRoll>();
      this.sewinQueue = Substitute.For<ISewinQueue>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbLocal = Substitute.For<IDbLocal>();
      this.appInfo = Substitute.For<IServiceSettings>();
      this.userAttentions = Substitute.For<IUserAttentions<MahloRoll>>();
      this.criticalStops = Substitute.For<ICriticalStops<MahloRoll>>();

      this.appInfo.MinRollLengthForStyleAndRollCounting = 1;
      this.appInfo.MinRollLengthForLengthChecking = 1;

      var stateProvider = Substitute.For<IProgramStateProvider>();
      stateProvider.GetProgramState().Returns("{}");
      this.programState = new ProgramState(stateProvider);

      var roll1 = new MahloService.Models.CarpetRoll()
      {
        Id = 1,
        RollNo = "12345",
        RollLength = roll1Length,
      };

      var roll2 = new MahloService.Models.CarpetRoll()
      {
        Id = 2,
        RollNo = "12346",
        RollLength = roll2Length,
      };

      var roll3 = new MahloService.Models.CarpetRoll()
      {
        Id = 3,
        RollNo = "12347",
        RollLength = roll3Length,
      };

      var roll4 = new MahloService.Models.CarpetRoll()
      {
        Id = 4,
        RollNo = "12348",
        RollLength = roll4Length,
      };

      var roll5 = new MahloService.Models.CarpetRoll()
      {
        Id = 5,
        RollNo = "12349",
        RollLength = roll5Length,
      };

      var roll6 = new MahloService.Models.CarpetRoll()
      {
        Id = 6,
        RollNo = "12350",
        RollLength = roll6Length,
      };

      var roll7 = new MahloService.Models.CarpetRoll()
      {
        Id = 7,
        RollNo = "12351",
        RollLength = roll7Length,
      };

      this.carpetRolls = new List<CarpetRoll> { roll1, roll2, roll3, roll4, roll5, roll6, roll7 };

      this.sewinQueue.Rolls.Returns(new BindingList<CarpetRoll>(carpetRolls));

      this.target = new MahloLogic(this.srcData, this.sewinQueue, this.appInfo, this.userAttentions, this.criticalStops, this.programState, this.testSchedulers);
      this.target.CurrentRoll = this.sewinQueue.Rolls[0];
      Assert.True(this.userAttentions.VerifyRollSequence);
      Assert.NotNull(this.target.CurrentRoll);
      this.userAttentions.ClearAll();
      this.target.Start();
    }

    public void Dispose()
    {
      this.target.Dispose();
    }

    [Fact]
    public void SpeedUpdatesWhenItChanges()
    {
      this.srcData.FeetPerMinute = 503;
      Assert.Equal(503, target.CurrentRoll.MalSpeed);
      Assert.Equal(503, target.Speed);
    }

    [Fact]
    public void IsMappingValidPasses()
    {
      Assert.True(target.GetIsMappingValid());
      this.userAttentions.Any.Returns(true);
      Assert.False(target.GetIsMappingValid());
      this.userAttentions.Any.Returns(false);

      this.criticalStops.Any.Returns(true);
      Assert.False(this.target.GetIsMappingValid());
      this.criticalStops.Any.Returns(false);

      this.target.CurrentRoll.RollNo = CarpetRoll.CheckRollId;
      Assert.False(this.target.GetIsMappingValid());
      this.target.CurrentRoll.RollNo = "SomethingElse";
      Assert.True(this.target.GetIsMappingValid());
    }

    [Fact]
    public void CurrentRollFeetTracksFeetChanges()
    {
      srcData.FeetCounter = 5;
      Assert.Equal(5, this.target.MeasuredLength);
    }

    [Fact]
    public void RollTooLongSetsUserAttention()
    {
      target.CurrentRoll = this.sewinQueue.Rolls[1];
      this.appInfo.RollTooLongThreshold = 1.1;

      double maxlength = target.CurrentRoll.RollLength * 1.1;
      srcData.FeetCounter = (int)maxlength;
      Assert.False(this.userAttentions.IsRollTooLong);
      srcData.FeetCounter = (int)maxlength + 1;
      Assert.True(this.userAttentions.IsRollTooLong);
    }

    [Fact]
    public void RollLengthGreaterThan90PercentOfExpectedRollLengthIsNotTooShort()
    {
      target.CurrentRoll = this.sewinQueue.Rolls[1];
      double minLength = target.CurrentRoll.RollLength * 0.9;
      srcData.FeetCounter = (int)minLength;
      srcData.IsSeamDetected = true;
      Assert.False(this.userAttentions.IsRollTooShort);
    }

    [Fact]
    public void RollLengthLessThan90PercentOfExpectedRollLengthIsTooShort()
    {
      this.appInfo.RollTooShortThreshold = 0.9;
      this.appInfo.RollTooLongThreshold = 1.1;
      this.appInfo.MinRollLengthForLengthChecking = 50;
      this.userAttentions.Any.Returns(false);
      this.criticalStops.Any.Returns(false);

      // The first roll arms things for the second roll to test
      target.CurrentRoll = this.sewinQueue.Rolls[1];
      srcData.FeetCounter = target.CurrentRoll.RollLength;
      srcData.IsSeamDetected = true;

      // The second roll
      this.userAttentions.IsRollTooShort = false;
      double minLength = target.CurrentRoll.RollLength * 0.9;
      srcData.FeetCounter = (int)minLength - 1;
      srcData.IsSeamDetected = false;
      srcData.IsSeamDetected = true;
      Assert.True(this.userAttentions.IsRollTooShort);
    }

    [Fact]
    public void SeamDetectedFalseIsIgnored()
    {
      srcData.FeetCounter = 500;
      srcData.IsSeamDetected = false;
      Assert.Equal(500, target.MeasuredLength);
    }

    [Fact]
    public void SeamDetectIsIgnoredIfSystemIsDisabled()
    {
      userAttentions.IsSystemDisabled = true;
      srcData.FeetCounter = 500;
      srcData.IsSeamDetected = true;
      Assert.Equal(500, target.MeasuredLength);
    }

    [Fact]
    public void SeamDetectedResetsRollLength()
    {
      srcData.FeetCounter = 500;
      srcData.IsSeamDetected = true;
      Assert.Equal(0, target.MeasuredLength);
    }

    [Fact]
    public void SeamDetectedResetsMeasuredLength()
    {
      target.MeasuredLength = 595;
      srcData.IsSeamDetected = true;
      Assert.Equal(0, target.MeasuredLength);
    }

    [Fact]
    public void MeterResetsAfterNewRollSelected()
    {
      this.carpetRolls[0].MalFeet = 1;
      this.carpetRolls[1].MalFeet = 2;

      this.srcData.FeetCounter = 500;
      Assert.Equal(500, this.carpetRolls[0].MalFeet);
      srcData.IsSeamDetected = true;
      srcData.FeetCounter += 10;
      Assert.Equal(500, carpetRolls[0].MalFeet);
      Assert.Equal(10, carpetRolls[1].MalFeet);
    }

    [Fact]
    public void SeamDetectedIsAcknowledged()
    {
      srcData.IsSeamDetected = true;
      Assert.Equal(1, srcData.ResetSeamDetectorCalled);
    }

    [Fact]
    public void SeamDetectedFiresRollFinishedFollowedByRollStarted()
    {
      bool rollFinishedSeen = false;
      bool rollStartedSeen = false;
      target.CurrentRoll = this.sewinQueue.Rolls[0];

      this.appInfo.SeamDetectableThreshold = 50;
      this.target.MeasuredLength = 50;
      using (Observable.FromEvent<Action<CarpetRoll>, CarpetRoll>(
        h => this.target.RollFinished += h,
        h => this.target.RollFinished -= h)
      .Subscribe(roll =>
      {
        Assert.False(rollStartedSeen);
        Assert.Equal(50, this.target.MeasuredLength);
        rollFinishedSeen = true;
      }))

      using (Observable.FromEvent<Action<CarpetRoll>, CarpetRoll>(
        h => this.target.RollStarted += h,
        h => this.target.RollStarted -= h)
      .Subscribe(roll =>
      {
        Assert.True(rollFinishedSeen);
        rollStartedSeen = true;
      }))
      {
        srcData.IsSeamDetected = true;
        Assert.True(rollStartedSeen);
      }
    }

    [Fact]
    public void SeamDetectedIsIgnoredWhenFeetLessThanSeamDetectedThreshold()
    {
      this.appInfo.SeamDetectableThreshold = 50;

      target.CurrentRoll = this.sewinQueue.Rolls[0];

      this.target.MeasuredLength = appInfo.SeamDetectableThreshold - 1;
      Observable.FromEvent<Action<CarpetRoll>, CarpetRoll>(
        h => this.target.RollFinished += h,
        h => this.target.RollFinished -= h)
      .Subscribe(roll => throw new Exception());

      Observable.FromEvent<Action<CarpetRoll>, CarpetRoll>(
        h => this.target.RollStarted += h,
        h => this.target.RollStarted -= h)
      .Subscribe(roll => throw new Exception());

      srcData.IsSeamDetected = true;

      // ResetSeamDetector should be called for every seam detection event
      Assert.Equal(1, this.srcData.ResetSeamDetectorCalled);
    }

    [Fact]
    public void NextRollSelectedWhenNewRollStarts()
    {
      Assert.Same(this.sewinQueue.Rolls[0], this.target.CurrentRoll);
      srcData.IsSeamDetected = true;
      Assert.Same(this.sewinQueue.Rolls[1], this.target.CurrentRoll);
    }

    [Fact]
    public void VerifyRollSequenceIsSetWhenConfiguredNumberOfRollAreProcessed()
    {
      this.userAttentions.VerifyRollSequence = false;
      this.appInfo.CheckAfterHowManyRolls = 3;
      this.appInfo.CheckAfterHowManyStyles = 1000;
      for (int j = 0; j < this.appInfo.CheckAfterHowManyRolls; j++)
      {
        ProduceRoll();
        Assert.False(this.userAttentions.VerifyRollSequence);
      }

      ProduceRoll();
      Assert.True(this.userAttentions.VerifyRollSequence);

      void ProduceRoll()
      {
        srcData.FeetCounter = this.target.CurrentRoll.RollLength;
        srcData.IsSeamDetected = false;
        srcData.IsSeamDetected = true;
        Assert.Equal(0, target.MeasuredLength);
      }
    }

    [Fact]
    public void VerifyRollSequeneIsSetWhenConfiguredNumberOfStylesAreProcessed()
    {
      this.userAttentions.VerifyRollSequence = false;
      this.appInfo.CheckAfterHowManyStyles = 3;
      this.appInfo.CheckAfterHowManyRolls = 100;
      this.carpetRolls[0].StyleCode = "style0";
      this.carpetRolls[1].StyleCode = "style1";
      this.carpetRolls[2].StyleCode = "style2";
      this.carpetRolls[3].StyleCode = "style2";
      this.carpetRolls[4].StyleCode = "style3";
      for (int j = 0; j < 4; j++)
      {
        ProduceRoll();
        Assert.False(this.userAttentions.VerifyRollSequence);
      }

      ProduceRoll();
      Assert.True(this.userAttentions.VerifyRollSequence);

      void ProduceRoll()
      {
        srcData.FeetCounter = this.target.CurrentRoll.RollLength;
        srcData.IsSeamDetected = false;
        srcData.IsSeamDetected = true;
        Assert.Equal(0, target.MeasuredLength);
      }
    }
  }
}