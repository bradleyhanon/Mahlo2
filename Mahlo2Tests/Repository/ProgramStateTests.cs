using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Repository
{
  public class ProgramStateTests
  {
    IProgramStateProvider provider;

    public ProgramStateTests()
    {
      provider = Substitute.For<IProgramStateProvider>();
      provider.GetProgramState().Returns("{}");
    }

    [Fact]
    public void TestConstructor()
    {
      dynamic obj = (dynamic)new JObject();
      obj.Good = "Bad";
      ProgramState state = new ProgramState(this.provider);
      state.Root.Good = "badder";
      state.Root.Stuff = new { rollId = 5, name = "Humpty Dumpty" };
      var s = JsonConvert.SerializeObject(state.Root);
    }


  }
}
