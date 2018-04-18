﻿using System;
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
    const string street = "101 1st Ave";
    const string city = "Chattanooga";
    IProgramStateProvider provider;

    public ProgramStateTests()
    {
      provider = Substitute.For<IProgramStateProvider>();
      provider.GetProgramState().Returns("{}");
    }
    
    [Fact]
    public void NonExistentValuesReturnNull()
    {
      dynamic state = new ProgramState(this.provider);
      Assert.Null(state.NotThere);
    }

    [Fact]
    public void TestGetProperties()
    {
      this.provider.GetProgramState().Returns("{ 'value':5, 'stuff':{ 'name':'smith', 'age':11 }}");
      dynamic state = new ProgramState(this.provider);
      int value = state.value;
      dynamic stuff = state.stuff;
      Assert.Equal(5, value);
      Assert.Equal("smith", (string)stuff.name);
      Assert.Equal(11, (int)stuff.age);
    }

    [Fact]
    public void TestSetProperties()
    {
      const string street = "101 1st Ave";
      const string city = "Chattanooga";
      dynamic state = new ProgramState(this.provider);
      state.BigMoney = 10000.55;
      state.Address = new { Street = street, City = city, Zip=12345 };
      Assert.Equal(10000.55, (double)state.BigMoney);
      Assert.Equal(street, (string)state.Address.Street);
      Assert.Equal(city, (string)state.Address.City);
      Assert.Equal(12345, (int)state.Address.Zip);
    }

    [Fact]
    public void PropertiesaAreAvailableByNameInString()
    {
      dynamic state = new ProgramState(this.provider);
      state["BigMoney"] = 10000;
      Assert.Equal(10000, (int)state["BigMoney"]);
    }

    [Fact]
    public void GetObjectWorks()
    {
      dynamic state = new ProgramState(this.provider);
      dynamic obj = state.GetObject("MeterLogic", "Mahlo");
      obj.value = 5;
      Assert.Equal(5, (int)state.MeterLogic.Mahlo.value);
    }

    [Fact]
    public void ResetClearsAllAndSetsDefaultProperties()
    {
      dynamic state = new ProgramState(this.provider);
      state.MahloRoll = new { rollId = 1 };
      Assert.NotNull(state.MahloRoll);
      state.Reset();
      Assert.Null(state.MahloRoll);
    }

    [Fact]
    public void RoundTripValuesArePreservedEvenForDefaultProperties()
    {
      string savedState = string.Empty;

      // Save the state string when SaveProgramState(s) called
      this.provider
        .When(x => x.SaveProgramState(Arg.Any<string>()))
        .Do(x => savedState = (string)x.Args()[0]);

      // Set state information and dispose the state object to save.
      var state1 = new ProgramState(this.provider);
      dynamic state = state1;
      state.Age = 55;
      state.MahloRoll = new { Street = street, City = city, Garbage = false };
      state.BowAndSkewRoll = new { rollId = 5 };
      state.PatternRepeatRoll = new { rollId = 4 };
      ((IDisposable)state1).Dispose();
      this.provider
        .Received(1);

      // Verify that the state can be reconstituted
      this.provider = Substitute.For<IProgramStateProvider>();
      this.provider.GetProgramState().Returns(savedState);
      dynamic state2 = new ProgramState(this.provider);
      Assert.Equal(55, (int)state2.Age);
      Assert.Equal(street, (string)state2.MahloRoll.Street);
      Assert.Equal(city, (string)state2.MahloRoll.City);
      Assert.False((bool)state2.MahloRoll.Garbage);

      Assert.Equal(5, (int)state2.BowAndSkewRoll.rollId);
      Assert.Equal(4, (int)state2.PatternRepeatRoll.rollId);
    }
  }
}
