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
  public class MeterLogicTests
  {
    private MockMeterSrc srcData;
    private ISewinQueue sewinQueue;
    private IDbMfg dbMfg;
    private IDbLocal dbLocal;
    private IAppInfoBAS appInfo;
    private IUserAttentions<MahloRoll> userAttentions;
    private ICriticalStops<MahloRoll> criticalStops;
    private IMockState programState;

    private MeterLogic<MahloRoll> target;

    public MeterLogicTests()
    {
      BindingList<GreigeRoll> rolls = new BindingList<GreigeRoll>();
      this.srcData = new MockMeterSrc();
      this.sewinQueue = Substitute.For<ISewinQueue>();
      this.dbMfg = Substitute.For<IDbMfg>();
      this.dbLocal = Substitute.For<IDbLocal>();
      this.appInfo = Substitute.For<IAppInfoBAS>();
      this.userAttentions = Substitute.For<IUserAttentions<MahloRoll>>();
      this.criticalStops = Substitute.For<ICriticalStops<MahloRoll>>();
      //this.programState = Substitute.For<IProgramState>();
      this.programState = Substitute.For<IMockState>();
      SubstituteExtensions.Returns(this.programState, )

      this.sewinQueue.Rolls.Returns(rolls);

      this.sewinQueue.Rolls.Add(new Mahlo.Models.GreigeRoll()
      {
        RollId = 1,
        RollNo = "12345",
        RollLength = 100,
      });
      this.sewinQueue.Rolls.Add(new Mahlo.Models.GreigeRoll()
      {
        RollId = 2,
        RollNo = "12346",
        RollLength = 100,
      });

      this.target = new MeterLogic<MahloRoll>(this.srcData, this.sewinQueue, this.appInfo, this.userAttentions, this.criticalStops, this.programState);
      this.target.Start();
    }

    interface IMockState
    {
      dynamic MeterLogic { get; set; }
    }

    [Fact]
    public void CurrentGreigeRollIsFirstSewinQueueRollByDefault()
    {
      Assert.Equal(this.sewinQueue.Rolls[0], target.CurrentGreigeRoll);
    }

    [Fact]
    public void CurrentRollFeetTracksFeetChanges()
    {
      srcData.FeetCounterSubject.OnNext(5);
      Assert.Equal(5, this.target.CurrentRoll.Feet);
    }

    //[Fact]
    //public void StatusIndicatorTurnsOffAfterSeamDetectableThreshold()
    //{
    //  srcData.MetersCount = 5;
    //  srcData.PropertyChanged += Raise.Event<PropertyChangedEventHandler>(this, new PropertyChangingEventArgs(nameof(srcData.SeamDetected)));

    //}
  }
}
