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
  public class CriticalStopsTests
  {
    private IMeterSrc<MahloRoll> meterSrc;
    private CriticalStops<MahloRoll> target;

    public CriticalStopsTests()
    {
      this.meterSrc = Substitute.For<IMeterSrc<MahloRoll>>();
      this.target = new CriticalStops<MahloRoll>(this.meterSrc);
      Assert.False(target.Any);
    }

    [Fact]
    public void TestMahloCommError()
    {
      this.target.IsMahloCommError = true;
      Assert.True(this.target.IsMahloCommError);
      Assert.True(this.target.Any);
      this.meterSrc.Received(1).SetCriticalAlarm(true);

      this.meterSrc.ClearReceivedCalls();
      this.target.IsMahloCommError = false;
      Assert.False(this.target.IsMahloCommError);
      Assert.False(this.target.Any);
      this.meterSrc.Received(1).SetCriticalAlarm(false);
    }

    [Fact]
    public void TestMahloPLCError()
    {
      this.target.IsPlcCommError = true;
      Assert.True(this.target.IsPlcCommError);
      Assert.True(this.target.Any);
      this.meterSrc.Received(1).SetCriticalAlarm(true);

      this.meterSrc.ClearReceivedCalls();
      this.target.IsPlcCommError = false;
      Assert.False(this.target.IsPlcCommError);
      Assert.False(this.target.Any);
      this.meterSrc.Received(1).SetCriticalAlarm(false);
    }

    [Fact]
    public void TestBoth()
    {
      this.target.IsPlcCommError = true;
      this.target.IsMahloCommError = true;
      Assert.True(this.target.IsPlcCommError);
      this.meterSrc.Received(1).SetCriticalAlarm(true);
      Assert.True(this.target.IsMahloCommError);
      this.meterSrc.Received(1).SetCriticalAlarm(true);

      this.meterSrc.ClearReceivedCalls();
      this.target.IsPlcCommError = false;
      Assert.True(this.target.Any);
      this.meterSrc.DidNotReceive().SetCriticalAlarm(Arg.Any<bool>());
      this.target.IsMahloCommError = false;
      Assert.False(this.target.Any);
      this.meterSrc.Received(1).SetCriticalAlarm(false);
    }
  }
}
