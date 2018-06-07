using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using MahloClient.Logic;
using Xunit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MahloClientTests.Logic
{
  public class SewinQueueTests : IEqualityComparer<CarpetRoll>
  {
    private SewinQueue target;

    public SewinQueueTests()
    {
      this.target = new SewinQueue();
      this.target.UpdateSewinQueue(ToJArray(this.GenerateRolls(100, 3)));
    }

    [Fact]

    public void StartingRollsAreAddedToEmptyQueue()
    {
      var expected = this.GenerateRolls(100, 3);
      Assert.True(expected.SequenceEqual(this.target.Rolls, this));
    }

    [Fact]
    public void NewRollsAreAddedAndOldRollsAreUpdated()
    {
      var newRolls = this.GenerateRolls(101, 3, 144);
      this.target.UpdateSewinQueue(ToJArray(newRolls));

      var expect = this.GenerateRolls(101, 3, 144);
      Assert.True(expect.SequenceEqual(this.target.Rolls, this));
    }

    //[Fact]
    //public void RollsAreReorderedToMatchNewRolls()
    //{
    //  var newRolls =
    //    GenerateRolls(101, 1)
    //    .Concat(GenerateRolls(103, 1))
    //    .Concat(GenerateRolls(102, 1))
    //    .Concat(GenerateRolls(104, 2));

    //  this.target.UpdateSewinQueue(newRolls);

    //  var expect = GenerateRolls(100,1).Concat(newRolls);
    //  Assert.True(expect.SequenceEqual(this.target.Rolls, this));
    //}

    [Fact]
    public void MovingExtremesWorks()
    {
      var oldRolls = GenerateRolls(100, 100);
      var newRolls = oldRolls.Skip(1).Concat(oldRolls.Take(1));
      this.target.Rolls.Clear();
      this.target.UpdateSewinQueue(ToJArray(oldRolls));
      this.target.UpdateSewinQueue(ToJArray(newRolls));
      Assert.True(newRolls.SequenceEqual(this.target.Rolls, this));
    }

    public bool Equals(CarpetRoll x, CarpetRoll y)
    {
      return x.RollNo == y.RollNo && x.RollWidth == y.RollWidth;
    }

    public int GetHashCode(CarpetRoll obj)
    {
      throw new NotImplementedException();
    }

    private IEnumerable<CarpetRoll> GenerateRolls(int startRollNo, int count, double width = 0)
    {
      var items = Observable.Range(startRollNo, count)
        .Select(index => new CarpetRoll() { RollNo = index.ToString(), RollWidth = width });

      return items.ToEnumerable().ToArray();
    }

    private JArray ToJArray(IEnumerable<object> src)
    {
      JArray result = new JArray();
      foreach(var item in src)
      {
        result.Add(JObject.FromObject(item));
      }

      return result;
    }
  }
}
