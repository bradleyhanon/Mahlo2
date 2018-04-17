using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo.Opc;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public class UserAttententionsTests
  {
    //VerifyRollSequence = 1,
    //  RollTooLong = 2,
    //  RollTooShort = 4,
    //  SystemDisabled = 8,
    //  All = VerifyRollSequence | RollTooLong | RollTooShort | SystemDisabled,

    IMeterSrc<MahloRoll> meterSrc;
    UserAttentions<MahloRoll> target;

    public UserAttententionsTests()
    {
      meterSrc = Substitute.For<IMeterSrc<MahloRoll>>();
      target = new UserAttentions<MahloRoll>(this.meterSrc);
      Assert.False(target.Any);
    }

    [Fact]
    public void TestVerifyRollSequence()
    {
      target.VerifyRollSequence = true;
      Assert.True(target.VerifyRollSequence);
      meterSrc.Received(1).SetStatusIndicator(true);

      target.VerifyRollSequence = false;
      Assert.False(target.VerifyRollSequence);
      meterSrc.Received(1).SetStatusIndicator(false);
    }

    [Fact]
    public void TestRollTooLong()
    {
      target.IsRollTooLong = true;
      Assert.True(target.IsRollTooLong);
      Assert.True(target.VerifyRollSequence);
      meterSrc.Received(1).SetStatusIndicator(true);

      meterSrc.ClearReceivedCalls();
      target.IsRollTooLong = false;
      Assert.False(target.IsRollTooLong);
      Assert.True(target.VerifyRollSequence);
      meterSrc.DidNotReceive().SetStatusIndicator(Arg.Any<bool>());
    }

    [Fact]
    public void TestRollTooShort()
    {
      target.IsRollTooShort = true;
      Assert.True(target.IsRollTooShort);
      Assert.True(target.VerifyRollSequence);
      meterSrc.Received(1).SetStatusIndicator(true);

      meterSrc.ClearReceivedCalls();
      target.IsRollTooShort = false;
      Assert.False(target.IsRollTooShort);
      Assert.True(target.VerifyRollSequence);
      meterSrc.DidNotReceive().SetStatusIndicator(Arg.Any<bool>());
    }

    [Fact]
    public void TestSystemDisabled()
    {
      target.IsSystemDisabled = true;
      Assert.True(target.IsSystemDisabled);
      Assert.True(target.VerifyRollSequence);
      meterSrc.Received(1).SetStatusIndicator(true);

      meterSrc.ClearReceivedCalls();
      target.IsSystemDisabled = false;
      Assert.False(target.IsSystemDisabled);
      Assert.True(target.VerifyRollSequence);
      meterSrc.DidNotReceive().SetStatusIndicator(Arg.Any<bool>());
    }

    [Fact]
    public void RollTooLongClearsRollTooShort()
    {
      target.IsRollTooShort = true;
      target.IsRollTooLong = true;
      Assert.True(target.IsRollTooLong);
      Assert.False(target.IsRollTooShort);
      meterSrc.Received(1).SetStatusIndicator(true);
    }

    [Fact]
    public void RollTooShortClearsRollTooLong()
    {
      target.IsRollTooLong = true;
      target.IsRollTooShort = true;
      Assert.True(target.IsRollTooShort);
      Assert.False(target.IsRollTooLong);
      meterSrc.Received(1).SetStatusIndicator(true);
    }

    [Fact]
    public void TestClearAll()
    {
      target.IsRollTooLong =
        target.IsRollTooShort =
        target.IsSystemDisabled = true;
      Assert.True(target.Any);
      meterSrc.Received(1).SetStatusIndicator(true);

      meterSrc.ClearReceivedCalls();
      target.ClearAll();
      Assert.False(target.Any);
      meterSrc.Received(1).SetStatusIndicator(false);
    }
  }
}
