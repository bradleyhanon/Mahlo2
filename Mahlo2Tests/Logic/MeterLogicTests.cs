using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;
using Mahlo2Tests.Mocks;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public sealed class MeterLogicTests : IDisposable
  {
    private MockMeterSrc srcData;
    private ISewinQueue sewinQueue;
    private IDbMfg dbMfg;
    private IDbLocal dbLocal;
    private IAppInfoBAS appInfo;
    private IUserAttentions<MahloRoll> userAttentions;
    private ICriticalStops<MahloRoll> criticalStops;
    private dynamic programState;

    private MeterLogic<MahloRoll> target;

    public MeterLogicTests()
    {
      const int roll1Length = 100;
      const int roll2Length = 200;
      const int roll3Length = 300;
      this.srcData = new MockMeterSrc();
      this.sewinQueue = Substitute.For<ISewinQueue>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbLocal = Substitute.For<IDbLocal>();
      this.appInfo = Substitute.For<IAppInfoBAS>();
      this.userAttentions = Substitute.For<IUserAttentions<MahloRoll>>();
      this.criticalStops = Substitute.For<ICriticalStops<MahloRoll>>();

      var stateProvider = Substitute.For<IProgramStateProvider>();
      stateProvider.GetProgramState().Returns("{}");
      this.programState = new ProgramState(stateProvider);

      var roll1 = new Mahlo.Models.GreigeRoll()
      {
        RollId = 1,
        RollNo = "12345",
        RollLength = roll1Length,
      };

      var roll2 = new Mahlo.Models.GreigeRoll()
      {
        RollId = 2,
        RollNo = "12346",
        RollLength = roll2Length,
      };

      var roll3 = new Mahlo.Models.GreigeRoll()
      {
        RollId = 3,
        RollNo = "12347",
        RollLength = roll3Length,
      };

      List<GreigeRoll> rolls = new List<GreigeRoll> { roll1, roll2, roll3 };

      this.sewinQueue.Rolls.Returns(new BindingList<GreigeRoll>(rolls));

      this.sewinQueue
        .TryGetRoll(Arg.Any<int>(), out GreigeRoll value)
        .Returns(x =>
        {
          x[1] = rolls[0];
          return true;
        });

      this.target = new MeterLogic<MahloRoll>(this.srcData, this.sewinQueue, this.appInfo, this.userAttentions, this.criticalStops, this.programState);
      Assert.True(this.userAttentions.VerifyRollSequence);
      Assert.NotNull(this.target.CurrentGreigeRoll);
      this.userAttentions.ClearAll();
      this.target.Start();
    }

    public void Dispose()
    {
      this.target.Dispose();
    }

    [Fact]
    public void IsMappingValidPasses()
    {
      Assert.True(target.IsMappingValid);
      this.userAttentions.Any.Returns(true);
      Assert.False(target.IsMappingValid);
      this.userAttentions.Any.Returns(false);

      this.criticalStops.Any.Returns(true);
      Assert.False(this.target.IsMappingValid);
      this.criticalStops.Any.Returns(false);

      this.target.CurrentGreigeRoll.RollNo = GreigeRoll.CheckRollId;
      Assert.False(this.target.IsMappingValid);
      this.target.CurrentGreigeRoll.RollNo = "SomethingElse";
      Assert.True(this.target.IsMappingValid);
    }

    [Fact]
    public void CurrentRollFeetTracksFeetChanges()
    {
      srcData.FeetCounterSubject.OnNext(5);
      Assert.Equal(5, this.target.CurrentRoll.Feet);
    }

    [Fact]
    public void RollTooLongSetsUserAttention()
    {
      target.CurrentGreigeRoll = this.sewinQueue.Rolls[1];

      double maxlength = target.CurrentGreigeRoll.RollLength * 1.1;
      srcData.FeetCounterSubject.OnNext((int)maxlength);
      Assert.False(this.userAttentions.IsRollTooLong);
      srcData.FeetCounterSubject.OnNext((int)maxlength + 1);
      Assert.True(this.userAttentions.IsRollTooLong);
    }

    [Fact]
    public void RollLengthGreaterThan90PercentOfExpectedRollLengthIsNotTooShort()
    {
      target.CurrentGreigeRoll = this.sewinQueue.Rolls[1];
      double minLength = target.CurrentGreigeRoll.RollLength * 0.9;
      srcData.FeetCounterSubject.OnNext((int)minLength);
      srcData.SeamDetectedSubject.OnNext(true);
      Assert.False(this.userAttentions.IsRollTooShort);
    }

    [Fact]
    public void RollLengthLessThan90PercentOfExpectedRollLengthIsTooShort()
    {
      target.CurrentGreigeRoll = this.sewinQueue.Rolls[1];
      double minLength = target.CurrentGreigeRoll.RollLength * 0.9;
      srcData.FeetCounterSubject.OnNext((int)minLength - 1);
      srcData.SeamDetectedSubject.OnNext(true);
      Assert.True(this.userAttentions.IsRollTooShort);
    }

    [Fact]
    public void SeamDetectedFalseIsIgnored()
    {
      srcData.FeetCounterSubject.OnNext(500);
      srcData.SeamDetectedSubject.OnNext(false);
      Assert.Equal(500, target.CurrentRoll.Feet);
    }

    [Fact]
    public void SeamDetectedResetsRollLength()
    {
      srcData.FeetCounterSubject.OnNext(500);
      srcData.SeamDetectedSubject.OnNext(true);
      Assert.Equal(0, target.CurrentRoll.Feet);
    }

    [Fact]
    public void SeamDetectedResetsMeterCount()
    {
      srcData.SeamDetectedSubject.OnNext(true);
      Assert.Equal(1, srcData.ResetMeterOffsetCalled);
    }

    [Fact]
    public void SeamDetectedIsAcknowledged()
    {
      srcData.SeamDetectedSubject.OnNext(true);
      Assert.Equal(1, srcData.ResetSeamDetectorCalled);
    }

    [Fact]
    public void SeamDetectedFiresRollFinishedFollowedByRollStarted()
    {
      bool rollFinishedSeen = false;
      bool rollStartedSeen = false;
      target.CurrentGreigeRoll = this.sewinQueue.Rolls[0];

      this.appInfo.SeamDetectableThreshold = 50;
      this.target.CurrentRoll.Feet = 50;
      this.target.RollFinished.Subscribe(roll =>
      {
        Assert.False(rollStartedSeen);
        Assert.Equal(50, this.target.CurrentRoll.Feet);
        rollFinishedSeen = true;
      });

      target.RollStarted.Subscribe(roll =>
      {
        Assert.True(rollFinishedSeen);
        rollStartedSeen = true;
      });

      srcData.SeamDetectedSubject.OnNext(true);
      Assert.True(rollStartedSeen);
    }

    [Fact]
    public void SeamDetectedIsIgnoredWhenFeetLessThanSeamDetectedThreshold()
    {
      this.appInfo.SeamDetectableThreshold = 50;

      target.CurrentGreigeRoll = this.sewinQueue.Rolls[0];

      this.target.CurrentRoll.Feet = appInfo.SeamDetectableThreshold - 1;
      this.target.RollFinished.Subscribe(roll => throw new Exception());

      target.RollStarted.Subscribe(roll => throw new Exception());

      srcData.SeamDetectedSubject.OnNext(true);

      // ResetSeamDetector should be called for every seam detection event
      Assert.Equal(1, this.srcData.ResetSeamDetectorCalled);
    }

    [Fact]
    public void NextGreigeRollSelectedWhenNewRollStarts()
    {
      var oldGreigeRoll = this.target.CurrentGreigeRoll;
      oldGreigeRoll.RollId = 1;
      srcData.SeamDetectedSubject.OnNext(true);
      this.sewinQueue.Received(1).TryGetRoll(oldGreigeRoll.RollId + 1, out GreigeRoll newGreigeRoll);
    }
  }
}
