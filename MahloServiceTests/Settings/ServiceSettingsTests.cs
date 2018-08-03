using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using Xunit;

namespace MahloServiceTests.Settings
{
  public class ServiceSettingsTests
  {
    [Fact]
    public void GetHLReturnsVinyl()
    {
      ServiceSettings settings = new ServiceSettings();

      Assert.Equal("Vinyl", settings.GetBackingSpec("hl").Backing);
    }

    [Fact]
    public void GetSAReturnsLatex()
    {
      var settings = new ServiceSettings();
      Assert.Equal("Latex", settings.GetBackingSpec("Sa").Backing);
    }

    [Fact]
    public void GetHRReturnsUrethane()
    {
      var settings = new ServiceSettings();
      Assert.Equal("Urethane", settings.GetBackingSpec("HR").Backing);
    }
  }
}
