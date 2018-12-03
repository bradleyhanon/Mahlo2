using System.Collections.Generic;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public class PatternRepeatLogicTests
  {
    private readonly IServiceSettings settings = Substitute.For<IServiceSettings>();
    private readonly GreigeRoll greigeRoll = new GreigeRoll();
    private readonly List<double> elongations = new List<double>();
    private readonly BackingSpec saSpec = new BackingSpec { Backing = "SA", MaxBow = 0.5, MaxSkew = 1.25, DlotSpec = 0.0100 };
    private readonly BackingSpec hlSpec = new BackingSpec { Backing = "HL", MaxBow = 0.25, MaxSkew = 0.75, DlotSpec = 0.0075 };

    public PatternRepeatLogicTests()
    {
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
        this.settings.GetBackingSpec("SA").Returns(this.saSpec);
        var tuple = saTable[j];
        Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("SA", tuple.gt + 0.001, this.settings));
        Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("SA", tuple.le, this.settings));

        this.settings.GetBackingSpec("HL").Returns(this.hlSpec);
        tuple = vinylTable[j];
        //Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("xx", tuple.gt + 0.001, settings));
        //Assert.Equal(tuple.dlot, PatternRepeatLogic.CalculateDlot("xx", tuple.le - 0.001, settings));
      }

      List<(double epe, string dlot)> list = new List<(double epe, string dlot)>();
      for (decimal eped = 0.950M; eped < 1.040M; eped += 0.001M)
      {
        list.Add(((double)eped, PatternRepeatLogic.CalculateDlot("HL", (double)eped, this.settings)));
      }
    }
  }
}
