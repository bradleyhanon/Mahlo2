using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloClient;
using MahloClient.Logic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace MahloClientTests
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

      //expect.CriticalStops.Any = true;
      expect.CriticalStops.IsPlcCommError = true;
      expect.CriticalStops.IsMahloCommError = true;

      string json = JsonConvert.SerializeObject(expect);
      Envelope actual = JsonConvert.DeserializeObject<Envelope>(json);

      Envelope2 populated = new Envelope2();
      Assert.False(populated.CriticalStops.IsMahloCommError);
      JToken token = JToken.Parse(json);
      token.Populate(populated);
      Assert.True(populated.CriticalStops.IsPlcCommError);
      Assert.True(populated.CriticalStops.IsMahloCommError);
      Assert.Equal("Howdy", populated.Item1);
    }

    internal class Envelope
    {
      public string Item1 { get; set; }
      public ICriticalStops CriticalStops { get; } = new CriticalStops<Envelope>();
    }

    internal class Envelope2
    {
      public ICriticalStops CriticalStops { get; } = new CriticalStops<Envelope>();
      public string Item1 { get; set; }
    }
  }
}
