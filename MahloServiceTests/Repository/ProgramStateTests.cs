using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;
using NSubstitute;
using Xunit;

namespace MahloServiceTests.Repository
{
  public class ProgramStateTests
  {
    const string street = "101 1st Ave";
    const string city = "Chattanooga";
    IProgramStateProvider provider;

    public ProgramStateTests()
    {
      this.provider = Substitute.For<IProgramStateProvider>();
      this.provider.GetProgramState().Returns("{}");
    }

    [Fact]
    public void NonExistentValuesReturnDefault()
    {
      var state = new ProgramState(this.provider);
      Assert.Null(state.Get<int?>("NotThere"));
    }

    [Fact]
    public void SimpleValueWorks()
    {
      var state = new ProgramState(this.provider);
      state.Set("one", 1);
      Assert.Equal(1, state.Get<int>("one"));
    }

    [Fact]
    public void TestGetProperties()
    {
      this.provider.GetProgramState().Returns("{ 'value':5, 'stuff':{ 'name':'smith', 'age':11 }}");
      var state = new ProgramState(this.provider);
      int value = state.Get<int>("value");
      var stuff = state.GetSubState("stuff");
      Assert.Equal(5, value);
      Assert.Equal("smith", stuff.Get<string>("name"));
      Assert.Equal(11, stuff.Get<int>("age"));
    }

    [Fact]
    public void TestSetProperties()
    {
      const string street = "101 1st Ave";
      const string city = "Chattanooga";
      var state = new ProgramState(this.provider);
      state.Set("BigMoney", 10000.55);
      state.Set("Address", new { Street = street, City = city, Zip = 12345 });
      Assert.Equal(10000.55, state.Get<double>("BigMoney"));
      var address = state.GetSubState("Address");
      Assert.Equal(street, address.Get<string>("Street"));
      Assert.Equal(city, address.Get<string>("City"));
      Assert.Equal(12345, address.Get<int>("Zip"));
    }

    [Fact]
    public void GettingAnObjectWorks()
    {
      BowAndSkewMapDatum datum = new BowAndSkewMapDatum() { FeetCounter = 5, Bow = 605 };
      var state = new ProgramState(this.provider);
      state.Set(nameof(datum), datum);

      BowAndSkewMapDatum roll2 = state.Get<BowAndSkewMapDatum>(nameof(datum));
      Assert.Equal(5, roll2.FeetCounter);
      Assert.Equal(605, roll2.Bow);
    }

    [Fact]
    public void RemoveAllClearsAll()
    {
      var state = new ProgramState(this.provider);
      state.Set(nameof(Mahlo2MapDatum), new { FeetCounter = 1 });
      Assert.NotNull(state.Get<Mahlo2MapDatum>(nameof(Mahlo2MapDatum)));
      state.RemoveAll();
      Assert.Null(state.Get<Mahlo2MapDatum>(nameof(Mahlo2MapDatum)));
    }

    [Fact]
    public void RoundTripValuesArePreserved()
    {
      string savedState = string.Empty;

      // Save the state string when SaveProgramState(s) called
      this.provider
        .When(x => x.SaveProgramState(Arg.Any<string>()))
        .Do(x => savedState = (string)x.Args()[0]);

      // Set state information and dispose the state object to save.
      var state = new ProgramState(this.provider);
      state.Set("Age", 55);
      state.Set("Address", new { Street = street, City = city, Garbage = false });
      state.Set(nameof(BowAndSkewMapDatum), new { FeetCounter = 5 });
      state.Set(nameof(PatternRepeatMapDatum), new { FeetCounter = 4 });
      state.Save();
      this.provider
        .Received(1);

      // Be sure that the state can be reconstituted
      this.provider = Substitute.For<IProgramStateProvider>();
      this.provider.GetProgramState().Returns(savedState);
      var state2 = new ProgramState(this.provider);
      Assert.Equal(55, state2.Get<int>("Age"));
      var address = state2.GetSubState("Address");
      Assert.Equal(street, address.Get<string>("Street"));
      Assert.Equal(city, address.Get<string>("City"));
      Assert.False(address.Get<bool>("Garbage"));

      Assert.Equal(5, state2.Get<BowAndSkewMapDatum>(nameof(BowAndSkewMapDatum)).FeetCounter);
      Assert.Equal(4, state2.Get<PatternRepeatMapDatum>(nameof(PatternRepeatMapDatum)).FeetCounter);
    }

    [Fact]
    public void RoundTripValuesWorkForNestedAnonymousClasses()
    {
      string savedState = string.Empty;

      // Save the state string when SaveProgramState(s) called
      this.provider
        .When(x => x.SaveProgramState(Arg.Any<string>()))
        .Do(x => savedState = (string)x.Args()[0]);

      // Set state information and dispose the state object to save.
      var state = new ProgramState(this.provider);
      state.Set("Settings", new
      {
        Age = 55,
        Mahlo2MapDatum = new { Street = street, City = city, Garbage = false },
        BowAndSkewMapDatum = new { FeetCounter = 5 },
        PatternRepeatMapDatum = new { FeetCounter = 4 },
      });

      state.Save();
      this.provider
        .Received(1);

      // Be sure that the state can be reconstituted
      this.provider = Substitute.For<IProgramStateProvider>();
      this.provider.GetProgramState().Returns(savedState);
      var state2 = new ProgramState(this.provider);
      var settings = state2.GetSubState("Settings");
      Assert.Equal(55, settings.Get<int>("Age"));
      Assert.Equal(street, settings.GetSubState("Mahlo2MapDatum").Get<string>("Street"));
      Assert.Equal(city, settings.GetSubState("Mahlo2MapDatum").Get<string>("City"));
      Assert.False(settings.GetSubState("Mahlo2MapDatum").Get<bool>("Garbage"));

      Assert.Equal(5, settings.Get<BowAndSkewMapDatum>(nameof(BowAndSkewMapDatum)).FeetCounter);
      Assert.Equal(4, settings.Get<PatternRepeatMapDatum>(nameof(PatternRepeatMapDatum)).FeetCounter);
    }
  }
}
