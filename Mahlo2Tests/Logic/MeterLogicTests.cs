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
using Newtonsoft.Json.Linq;
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
    private IUserAttentions<MahloRoll> userAttentions;
    private ICriticalStops<MahloRoll> criticalStops;

    private MeterLogic<MahloRoll> target;

    public MeterLogicTests()
    {
      BindingList<GreigeRoll> rolls = new BindingList<GreigeRoll>();
      this.srcData = Substitute.For<IMahloSrc>();
      this.sewinQueue = Substitute.For<ISewinQueue>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbLocal = Substitute.For<IDbLocal>();
      this.appInfo = Substitute.For<IAppInfoBAS>();
      this.userAttentions = Substitute.For<IUserAttentions<MahloRoll>>();
      this.criticalStops = Substitute.For<ICriticalStops<MahloRoll>>();

      this.sewinQueue.Rolls.Returns(rolls);

      this.sewinQueue.Rolls.Add(new Mahlo.Models.GreigeRoll()
      {
        RollId = 1,
        G2ROLL = "12345",
        G2LTF = 100,
      });

      this.target = new MeterLogic<MahloRoll>(this.srcData, this.sewinQueue, this.appInfo, this.userAttentions, this.criticalStops);
      this.target.Start();
    }

    [Fact]
    public void TestJson()
    {
      JObject settings = new JObject();
      int rollId = (int?)settings["rollId"] ?? 0;
      

    }

    //[Fact]
    //public void CurrentRollMetersTracksSrcMetersCount()
    //{
    //  srcData.MetersCount = 5;
    //  srcData.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangedEventArgs(nameof(srcData.MetersCount)));
    //  Assert.Equal(5, mahloLogic.CurrentRoll.Feet);
    //}

    //[Fact]
    //public void StatusIndicatorTurnsOffAfterSeamDetectableThreshold()
    //{
    //  srcData.MetersCount = 5;
    //  srcData.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangingEventArgs(nameof(srcData.SeamDetected)));

    //}
  }
}
