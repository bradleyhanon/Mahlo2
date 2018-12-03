using MahloService.Models;
using Xunit;

namespace MahloServiceTests.Models
{
  public class GreigeRollTests
  {
    [Fact]
    private void SwapWithTest()
    {
      GreigeRoll roll1 = new GreigeRoll
      {
        Id = 1,
        RollNo = "Roll1",
        OrderNo = "Order1",
        StyleCode = "StyleCode1",
        StyleName = "StyleName1",
        ColorCode = "ColorCode1",
        ColorName = "ColorName1",
        BackingCode = "BackingCode1",
        RollLength = 1,
        RollWidth = 1.1,
        DefaultRecipe = "DefaultRecipe1",
        PatternRepeatLength = 1.001,
        ProductImageURL = "ProductImageURL1",
      };

      GreigeRoll roll2 = new GreigeRoll
      {
        Id = 2,
        RollNo = "Roll2",
        OrderNo = "Order2",
        StyleCode = "StyleCode2",
        StyleName = "StyleName2",
        ColorCode = "ColorCode2",
        ColorName = "ColorName2",
        BackingCode = "BackingCode2",
        RollLength = 2,
        RollWidth = 2.2,
        DefaultRecipe = "DefaultRecipe2",
        PatternRepeatLength = 2.002,
        ProductImageURL = "ProductImageURL2",
      };

      // Id shouldn't swap, the others should swap
      roll1.SwapWith(roll2);

      Assert.Equal(2, roll2.Id);
      Assert.Equal("Roll1", roll2.RollNo);
      Assert.Equal("Order1", roll2.OrderNo);
      Assert.Equal("StyleCode1", roll2.StyleCode);
      Assert.Equal("StyleName1", roll2.StyleName);
      Assert.Equal("ColorCode1", roll2.ColorCode);
      Assert.Equal("ColorName1", roll2.ColorName);
      Assert.Equal("BackingCode1", roll2.BackingCode);
      Assert.Equal(1, roll2.RollLength);
      Assert.Equal(1.1, roll2.RollWidth);
      Assert.Equal("DefaultRecipe1", roll2.DefaultRecipe);
      Assert.Equal(1.001, roll2.PatternRepeatLength);
      Assert.Equal("ProductImageURL1", roll2.ProductImageURL);

      Assert.Equal(1, roll1.Id);
      Assert.Equal("Roll2", roll1.RollNo);
      Assert.Equal("Order2", roll1.OrderNo);
      Assert.Equal("StyleCode2", roll1.StyleCode);
      Assert.Equal("StyleName2", roll1.StyleName);
      Assert.Equal("ColorCode2", roll1.ColorCode);
      Assert.Equal("ColorName2", roll1.ColorName);
      Assert.Equal("BackingCode2", roll1.BackingCode);
      Assert.Equal(2, roll1.RollLength);
      Assert.Equal(2.2, roll1.RollWidth);
      Assert.Equal("DefaultRecipe2", roll1.DefaultRecipe);
      Assert.Equal(2.002, roll1.PatternRepeatLength);
      Assert.Equal("ProductImageURL2", roll1.ProductImageURL);
    }
  }
}
