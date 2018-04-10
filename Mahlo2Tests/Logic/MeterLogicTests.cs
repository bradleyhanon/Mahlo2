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
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public class MeterLogicTests
  {
    private IMahloSrc srcData;
    private ISewinQueue sewinQueue;
    private IDbMfg dbMfg;
    private IDbLocal dbLocal;
    private IAppInfoBAS appInfo;

    private MahloLogic mahloLogic;

    public MeterLogicTests()
    {
      this.srcData = Substitute.For<IMahloSrc>();
      this.sewinQueue = Substitute.For<ISewinQueue>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbLocal = Substitute.For<IDbLocal>();
      this.appInfo = Substitute.For<IAppInfoBAS>();

      this.sewinQueue.Rolls.Add(new Mahlo.Models.GreigeRoll()
      {
        RollId = 1,
        G2ROLL = "12345",
        G2LTF = 100,
      });

      this.mahloLogic = new MahloLogic(this.sewinQueue, this.srcData, this.dbMfg, this.dbLocal, this.appInfo);
    }

    [Fact]
    public void CurrentRollMetersTracksSrcMetersCount()
    {
      srcData.MetersCount = 5;
      srcData.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs(nameof(srcData.MetersCount)));
      Assert.Equal(5, mahloLogic.CurrentRoll.Meters);
    }
  }
}
