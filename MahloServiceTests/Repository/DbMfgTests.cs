using MahloService.Repository;

namespace MahloServiceTests.Repository
{
  public class DbMfgTests
  {
    private readonly DbMfg target;

    public DbMfgTests()
    {
      var factoryFactory = new DbConnectionFactory.Factory();
      this.target = new DbMfg(factoryFactory);
    }

    //[Fact]
    //public async void TryGetCutRollFromHost()
    //{
    //  await target.GetCutRollFromHost();
    //}
  }
}
