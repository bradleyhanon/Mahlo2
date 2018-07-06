using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public class PatternRepeatLogicTests
  {
    IServiceSettings settings = Substitute.For<IServiceSettings>();
    GreigeRoll greigeRoll = new GreigeRoll();
    List<double> elongations = new List<double>();

    public PatternRepeatLogicTests()
    {
      settings.EpeSpecSA = 0.01;
      settings.EpeSpecVinyl = 0.0075;
    }

    [Fact]
    public void CalculateDlotFollowsChart()
    {
      (double gt, double le, string dlot)[] saTable =
      {
        (0.000, 0.955, "+5"),
        (0.955, 0.965, "+4"),
        (0.965, 0.975, "+3"),
        (0.975, 0.985, "+2"),
        (0.985, 0.995, "+1"),
        (0.995, 1.005, "0"),
        (1.005, 1.015, "-1"),
        (1.015, 1.025, "-2"),
        (1.025, 1.035, "-3"),
        (1.035, 1.045, "-4"),
        (1.045, 9.999, "-5")
      };

      (double gt, double le, string dlot)[] vinylTable =
      {
        (0.000, 0.966, "+5"),
        (0.966, 0.974, "+4"),
        (0.974, 0.981, "+3"),
        (0.981, 0.989, "+2"),
        (0.989, 0.996, "+1"),
        (0.996, 1.004, "0"),
        (1.004, 1.011, "-1"),
        (1.011, 1.019, "-2"),
        (1.019, 1.026, "-3"),
        (1.026, 1.034, "-4"),
        (1.034, 9.999, "-5")
      };

      for (int j = 0; j < saTable.Length; j++)
      {
        var tuple = saTable[j];
        Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("SA", tuple.gt + 0.001, settings));
        Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("SA", tuple.le, settings));

        tuple = vinylTable[j];
        //Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("xx", tuple.gt + 0.001, settings));
        //Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("xx", tuple.le, settings));
      }
    }
  }
}
