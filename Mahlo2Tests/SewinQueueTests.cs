using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo.Repository;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests
{
  public class SewinQueueTests
  {
    GreigeRoll[] rolls1 =
    {
      new GreigeRoll() { G2ROLL = "100" },
      new GreigeRoll() { G2ROLL = "200" },
      new GreigeRoll() { G2ROLL = "300" },
    };

    IDbLocal dbLocal = Substitute.For<IDbLocal>();
    IDbMfg dbMfg = Substitute.For<IDbMfg>();

    SewinQueue sewinQueue;

    [Fact]
    public void Test()
    {
      dbMfg.GetIsSewinQueueChanged(0, string.Empty, string.Empty).Returns<bool>(false);
      sewinQueue = new SewinQueue(dbLocal, dbMfg);
      dbMfg.Received().GetIsSewinQueueChanged(0, string.Empty, string.Empty);
    }
  }
}
