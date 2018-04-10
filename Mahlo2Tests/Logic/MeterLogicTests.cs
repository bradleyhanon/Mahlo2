using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Logic;
using Mahlo.Opc;
using Mahlo.Repository;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public class MeterLogicTests
  {
    private IMeterSrc srcData;
    private ISewinQueue sewinQueue;
    private IDbMfg dbMfg;
    private IDbLocal dbLocal;
    private IAppInfoBAS appInfo;

    private MeterLogic meterlogic;

    public MeterLogicTests()
    {
      this.srcData = Substitute.For<IMeterSrc>();
      this.sewinQueue = Substitute.For<ISewinQueue>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbLocal = Substitute.For<IDbLocal>();
      this.appInfo = Substitute.For<IAppInfoBAS>();
      this.meterlogic = new MeterLogic(this.srcData, this.sewinQueue, this.dbMfg, this.dbLocal, this.appInfo);
    }

    [Fact]
    public void CurrentRollMetersTracksSrcMetersCount()
    {
      srcData.MetersCount = 5;
      srcData.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs(nameof(srcData.MetersCount)));
      Assert.Equal(5, meterlogic.CurrentRoll.Meters);
    }
  }
}
