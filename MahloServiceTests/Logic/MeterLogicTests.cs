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
using Microsoft.Reactive.Testing;

namespace MahloServiceTests.Logic
{
  public sealed class MeterLogicTests : IDisposable
  {
    private readonly MockMeterSrc<MahloModel> srcData;
    private readonly ISewinQueue sewinQueue;
    private readonly IDbMfg dbMfg;
    private readonly IDbLocal dbLocal;
    private readonly IServiceSettings appInfo;
    private readonly IUserAttentions<MahloModel> userAttentions;
    private readonly ICriticalStops<MahloModel> criticalStops;
    private readonly TestScheduler testSchedulers = new TestScheduler();
    private readonly dynamic programState;

    private readonly MeterLogic<MahloModel> target;
    private readonly List<GreigeRoll> greigeRolls;

    public MeterLogicTests()
    {
      const int roll1Length = 100;
      const int roll2Length = 200;
      const int roll3Length = 300;
      const int roll4Length = 400;
      const int roll5Length = 400;
      const int roll6Length = 400;
      const int roll7Length = 400;

      this.srcData = new MockMeterSrc<MahloModel>();
      this.sewinQueue = Substitute.For<ISewinQueue>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbLocal = Substitute.For<IDbLocal>();
      this.appInfo = Substitute.For<IServiceSettings>();
      this.userAttentions = Substitute.For<IUserAttentions<MahloModel>>();
      this.criticalStops = Substitute.For<ICriticalStops<MahloModel>>();

      this.appInfo.MinRollLengthForStyleAndRollCounting = 1;
      this.appInfo.MinRollLengthForLengthChecking = 1;

      var stateProvider = Substitute.For<IProgramStateProvider>();
      stateProvider.GetProgramState().Returns("{}");
      this.programState = new ProgramState(stateProvider);

      var roll1 = new MahloService.Models.GreigeRoll()
      {
        Id = 1,
        RollNo = "12345",
        RollLength = roll1Length,
      };

      var roll2 = new MahloService.Models.GreigeRoll()
      {
        Id = 2,
        RollNo = "12346",
        RollLength = roll2Length,
      };

      var roll3 = new MahloService.Models.GreigeRoll()
      {
        Id = 3,
        RollNo = "12347",
        RollLength = roll3Length,
      };

      var roll4 = new MahloService.Models.GreigeRoll()
      {
        Id = 4,
        RollNo = "12348",
        RollLength = roll4Length,
      };

      var roll5 = new MahloService.Models.GreigeRoll()
      {
        Id = 5,
        RollNo = "12349",
        RollLength = roll5Length,
      };

      var roll6 = new MahloService.Models.GreigeRoll()
      {
        Id = 6,
        RollNo = "12350",
        RollLength = roll6Length,
      };

      var roll7 = new MahloService.Models.GreigeRoll()
      {
        Id = 7,
        RollNo = "12351",
        RollLength = roll7Length,
      };

      this.greigeRolls = new List<GreigeRoll> { roll1, roll2, roll3, roll4, roll5, roll6, roll7 };

      this.sewinQueue.Rolls.Returns(new BindingList<GreigeRoll>(this.greigeRolls));

      this.target = new MahloLogic(this.dbLocal, this.srcData, this.sewinQueue, this.appInfo, this.userAttentions, this.criticalStops, this.programState, this.testSchedulers)
      {
        CurrentRoll = this.sewinQueue.Rolls[0]
      };

      this.sewinQueue.QueueChanged += Raise.Event<Action>(); 
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
      Assert.Equal(503, this.target.CurrentRoll.MalSpeed);
      Assert.Equal(503, this.target.Speed);
    }

    [Fact]
    public void IsMappingValidPasses()
    {
      Assert.True(this.target.GetIsMappingValid());
      this.userAttentions.Any.Returns(true);
      Assert.False(this.target.GetIsMappingValid());
      this.userAttentions.Any.Returns(false);

      this.criticalStops.Any.Returns(true);
      Assert.False(this.target.GetIsMappingValid());
      this.criticalStops.Any.Returns(false);

      this.target.CurrentRoll.RollNo = GreigeRoll.CheckRollId;
      Assert.False(this.target.GetIsMappingValid());
      this.target.CurrentRoll.RollNo = "SomethingElse";
      Assert.True(this.target.GetIsMappingValid());
    }

    [Fact]
    public void CurrentRollFeetTracksFeetChanges()
    {
      this.srcData.FeetCounter = 5;
      Assert.Equal(5, this.target.MeasuredLength);
    }

    [Fact]
    public void RollTooLongSetsUserAttention()
    {
      this.target.CurrentRoll = this.sewinQueue.Rolls[1];
      this.appInfo.RollTooLongFactor = 1.1;

      double maxlength = this.target.CurrentRoll.RollLength * 1.1;
      this.srcData.FeetCounter = (int)maxlength;
      Assert.False(this.userAttentions.IsRollTooLong);
      this.srcData.FeetCounter = (int)maxlength + 1;
      Assert.True(this.userAttentions.IsRollTooLong);
    }

    [Fact]
    public void RollLengthGreaterThan90PercentOfExpectedRollLengthIsNotTooShort()
    {
      this.target.CurrentRoll = this.sewinQueue.Rolls[1];
      double minLength = this.target.CurrentRoll.RollLength * 0.9;
      this.srcData.FeetCounter = (int)minLength;
      this.srcData.IsSeamDetected = true;
      Assert.False(this.userAttentions.IsRollTooShort);
    }

    [Fact]
    public void RollLengthLessThan90PercentOfExpectedRollLengthIsTooShort()
    {
      this.appInfo.RollTooShortFactor = 0.9;
      this.appInfo.RollTooLongFactor = 1.1;
      this.appInfo.MinRollLengthForLengthChecking = 50;
      this.userAttentions.Any.Returns(false);
      this.criticalStops.Any.Returns(false);

      // The first roll arms things for the second roll to test
      this.target.CurrentRoll = this.sewinQueue.Rolls[1];
      this.srcData.FeetCounter = this.target.CurrentRoll.RollLength;
      this.srcData.IsSeamDetected = true;

      // The second roll
      this.userAttentions.IsRollTooShort = false;
      double minLength = this.target.CurrentRoll.RollLength * 0.9;
      this.srcData.FeetCounter = (int)minLength - 1;
      this.srcData.IsSeamDetected = false;
      this.srcData.IsSeamDetected = true;
      Assert.True(this.userAttentions.IsRollTooShort);
    }

    [Fact]
    public void SeamDetectedFalseIsIgnored()
    {
      this.srcData.FeetCounter = 500;
      this.srcData.IsSeamDetected = false;
      Assert.Equal(500, this.target.MeasuredLength);
    }

    [Fact]
    public void SeamDetectIsIgnoredIfSystemIsDisabled()
    {
      this.userAttentions.IsSystemDisabled = true;
      this.srcData.FeetCounter = 500;
      this.srcData.IsSeamDetected = true;
      Assert.Equal(500, this.target.MeasuredLength);
    }

    [Fact]
    public void SeamDetectedResetsRollLength()
    {
      this.srcData.FeetCounter = 500;
      this.srcData.IsSeamDetected = true;
      Assert.Equal(0, this.target.MeasuredLength);
    }

    [Fact]
    public void SeamDetectedResetsMeasuredLength()
    {
      this.target.FeetCounterStart = 50;
      this.target.FeetCounterEnd = this.target.FeetCounterStart + 595;
      this.srcData.IsSeamDetected = true;
      Assert.Equal(0, this.target.MeasuredLength);
    }

    [Fact]
    public void MeterResetsAfterNewRollSelected()
    {
      this.greigeRolls[0].MalFeetCounterEnd = 1;
      this.greigeRolls[1].MalFeetCounterEnd = 2;

      this.srcData.FeetCounter = 500;
      Assert.Equal(500, this.greigeRolls[0].MalFeet);
      this.srcData.IsSeamDetected = true;
      this.srcData.FeetCounter += 10;
      Assert.Equal(500, this.greigeRolls[0].MalFeet);
      Assert.Equal(10, this.greigeRolls[1].MalFeet);
    }

    [Fact]
    public void SeamDetectedIsAcknowledgedAtMinSeamSpacing()
    {
      this.appInfo.MinSeamSpacing = 4;
      this.srcData.IsSeamDetected = true;
      this.srcData.FeetCounter += this.appInfo.MinSeamSpacing - 1;
      Assert.Equal(0, this.srcData.AcknowledgeSeamDetectCalled);
      this.srcData.FeetCounter++;
      Assert.Equal(1, this.srcData.AcknowledgeSeamDetectCalled);
    }

    [Fact]
    public void CheckRollFinishedWhenEndCheckRollPieceSeen()
    {
      var roll = this.target.CurrentRoll;
      roll.RollNo = GreigeRoll.CheckRollId;
      this.appInfo.MaxEndCheckRollPieceLength = 10;
      this.appInfo.MinSeamSpacing = 4;
      for (int j = 0; j < 10; j++)
      {
        this.srcData.FeetCounter += this.appInfo.MaxEndCheckRollPieceLength + 1;
        this.srcData.IsSeamDetected = true;
        this.srcData.IsSeamDetected = false;
        Assert.Equal(roll, this.target.CurrentRoll);
      }

      this.srcData.FeetCounter += this.appInfo.MaxEndCheckRollPieceLength;
      this.srcData.IsSeamDetected = true;
      this.srcData.IsSeamDetected = false;
      Assert.NotEqual(roll, this.target.CurrentRoll);
    }

    [Fact]
    public void SeamDetectedFiresRollFinishedFollowedByRollStarted()
    {
      bool rollFinishedSeen = false;
      bool rollStartedSeen = false;
      this.target.CurrentRoll = this.sewinQueue.Rolls[0];

      this.appInfo.SeamDetectableThreshold = 50;
      this.target.FeetCounterEnd = 50;
      using (Observable.FromEvent<Action<GreigeRoll>, GreigeRoll>(
        h => this.target.RollFinished += h,
        h => this.target.RollFinished -= h)
      .Subscribe(roll =>
      {
        Assert.False(rollStartedSeen);
        Assert.Equal(50, this.target.MeasuredLength);
        rollFinishedSeen = true;
      }))

      using (Observable.FromEvent<Action<GreigeRoll>, GreigeRoll>(
        h => this.target.RollStarted += h,
        h => this.target.RollStarted -= h)
      .Subscribe(roll =>
      {
        Assert.True(rollFinishedSeen);
        rollStartedSeen = true;
      }))
      {
        this.srcData.IsSeamDetected = true;
        Assert.True(rollStartedSeen);
      }
    }

    [Fact]
    public void NextRollSelectedWhenNewRollStarts()
    {
      Assert.Same(this.sewinQueue.Rolls[0], this.target.CurrentRoll);
      this.srcData.IsSeamDetected = true;
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
        this.srcData.FeetCounter = this.target.CurrentRoll.RollLength;
        this.srcData.IsSeamDetected = false;
        this.srcData.IsSeamDetected = true;
        Assert.Equal(0, this.target.MeasuredLength);
      }
    }

    [Fact]
    public void VerifyRollSequeneIsSetWhenConfiguredNumberOfStylesAreProcessed()
    {
      this.userAttentions.VerifyRollSequence = false;
      this.appInfo.CheckAfterHowManyStyles = 3;
      this.appInfo.CheckAfterHowManyRolls = 100;
      this.greigeRolls[0].StyleCode = "style0";
      this.greigeRolls[1].StyleCode = "style1";
      this.greigeRolls[2].StyleCode = "style2";
      this.greigeRolls[3].StyleCode = "style2";
      this.greigeRolls[4].StyleCode = "style3";
      for (int j = 0; j < 4; j++)
      {
        ProduceRoll();
        Assert.False(this.userAttentions.VerifyRollSequence);
      }

      ProduceRoll();
      Assert.True(this.userAttentions.VerifyRollSequence);

      void ProduceRoll()
      {
        this.srcData.FeetCounter = this.target.CurrentRoll.RollLength;
        this.srcData.IsSeamDetected = false;
        this.srcData.IsSeamDetected = true;
        Assert.Equal(0, this.target.MeasuredLength);
      }
    }
  }
}