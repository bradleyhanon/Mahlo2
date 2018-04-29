using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Repository;
using Xunit;

namespace Mahlo2Tests.Repository
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
