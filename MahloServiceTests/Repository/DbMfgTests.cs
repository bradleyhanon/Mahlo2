using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Repository;
using Xunit;

namespace MahloServiceTests.Repository
{
  public class DbMfgTests
  {
    private DbMfg target;
    public DbMfgTests()
    {
      var factoryFactory = new DbConnectionFactory.Factory();
      target = new DbMfg(factoryFactory);
    }

    //[Fact]
    //public async void TryGetCutRollFromHost()
    //{
    //  await target.GetCutRollFromHost();
    //}
  }
}
