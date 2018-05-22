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
  public class PatternRepeatLogicTests
  {
    MockMeterSrc dataSrc;
    IMeterLogic<PatternRepeatRoll> meterLogic;
    PatternRepeatLogic target;

    public PatternRepeatLogicTests()
    {
      this.dataSrc = new MockMeterSrc();
      this.meterLogic = Substitute.For<IMeterLogic<PatternRepeatRoll>>();
      this.target = new PatternRepeatLogic(this.dataSrc, this.meterLogic);

      this.meterLogic.CurrentRoll.Returns(new PatternRepeatRoll());
    }

    [Fact]
    public void ElongationValueFollowsChanges()
    {
      this.dataSrc.PatternRepeatSubject.OnNext(2.5);
      Assert.Equal(2.5, this.meterLogic.CurrentRoll.Elongation);
    }
  }
}
