namespace Mahlo2Tests.Logic
{
  public class BowAndSkewLogicTests
  {
    MockMeterSrc dataSrc;
    IMeterLogic<BowAndSkewRoll> meterLogic;
    BowAndSkewLogic target;

    public BowAndSkewLogicTests()
    {
      this.dataSrc = new MockMeterSrc();
      this.meterLogic = Substitute.For<IMeterLogic<BowAndSkewRoll>>();
      this.target = new BowAndSkewLogic(this.dataSrc, this.meterLogic);

      this.meterLogic.CurrentRoll.Returns(new BowAndSkewRoll());
    }

    [Fact]
    public void BowValueFollowsChanges()
    {
      this.dataSrc.BowSubject.OnNext(0.05);
      Assert.Equal(0.05, this.meterLogic.CurrentRoll.Bow);
    }

    [Fact]
    public void SkewValueFolowsChanges()
    {
      this.dataSrc.SkewSubject.OnNext(0.045);
      Assert.Equal(0.045, this.meterLogic.CurrentRoll.Skew);
    }
  }
}
