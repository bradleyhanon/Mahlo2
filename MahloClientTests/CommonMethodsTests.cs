using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService;
using Xunit;

namespace MahloClientTests
{
  public class CommonMethodsTests
  {
    [Fact]
    public void FeetToStrTest()
    {
      (double value, string expect)[] tuples = new(double value, string expect)[]
      {
        (144, "12' 0\""),
        (145, "12' 1\""),
        (146, "12' 2\""),
        (147, "12' 3\""),
        (148, "12' 4\""),
        (149, "12' 5\""),
        (150, "12' 6\""),
        (151, "12' 7\""),
        (152, "12' 8\""),
        (153, "12' 9\""),
        (154, "12' 10\""),
        (155, "12' 11\""),
        (156, "13' 0\""),
      };

      foreach(var tuple in tuples)
      {
        Assert.Equal(tuple.expect, CommonMethods.WidthToStr(tuple.value));
      }
    }
  }
}
