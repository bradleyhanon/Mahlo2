using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo2Tests.Mocks;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public class MahloLogicTests
  {
    MockMeterSrc dataSrc;
    IMeterLogic<MahloRoll> meterLogic;
    MahloLogic target;

    public MahloLogicTests()
    {
      this.dataSrc = new MockMeterSrc();
      this.meterLogic = Substitute.For<IMeterLogic<MahloRoll>>();
      this.target = new MahloLogic(this.dataSrc, this.meterLogic);

      this.meterLogic.CurrentRoll.Returns(new MahloRoll());
    }

    [Fact]
    public void ThereIsNothingToTestYet()
    {

    }
  }
}
