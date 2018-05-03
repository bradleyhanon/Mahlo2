using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapperClient;
using MapperClient.Logic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MapperClientTests
{
  public class JsonTests
  {
    [Fact]
    public void NestedClassesShouldPopulate()
    {
      Envelope expect = new Envelope
      {
        Item1 = "Howdy"
      };

      expect.CriticalStops.IsMahloCommError = true;

      string json = JsonConvert.SerializeObject(expect);
      Envelope actual = JsonConvert.DeserializeObject<Envelope>(json);

      Envelope populated = new Envelope();
      JToken token = JToken.Parse(json);
      token.Populate(populated);

    }

    internal class Envelope
    {
      public string Item1 { get; set; }
      public CriticalStops CriticalStops { get; } = new CriticalStops();
    }
  }
}
