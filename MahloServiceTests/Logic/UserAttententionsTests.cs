using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using MahloService.Logic;
using MahloService.Models;
using Xunit;

namespace MahloServiceTests.Logic
{
  public class UserAttententionsTests : IDisposable
  {
    //VerifyRollSequence = 1,
    //  RollTooLong = 2,
    //  RollTooShort = 4,
    //  SystemDisabled = 8,
    //  All = VerifyRollSequence | RollTooLong | RollTooShort | SystemDisabled,

    private bool anyChanged;
    private int anyChangesToTrue;
    private int anyChangesToFalse;
    private UserAttentions<MahloModel> target;
    private readonly IDisposable subscription;

    public UserAttententionsTests()
    {
      this.target = new UserAttentions<MahloModel>();

      this.subscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.target).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.target).PropertyChanged -= h)
          .Where(arg => arg.EventArgs.PropertyName == nameof(UserAttentions<MahloModel>.Any))
          .Subscribe(_ => this.anyChanged = (this.target.Any ? ++this.anyChangesToTrue : ++this.anyChangesToFalse) != 0);

      Assert.False(this.target.Any);
    }

    public void Dispose()
    {
      this.subscription.Dispose();
    }

    [Fact]
    public void TestVerifyRollSequence()
    {
      this.target.VerifyRollSequence = true;
      Assert.True(this.target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      this.target.VerifyRollSequence = false;
      Assert.False(this.target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToFalse);
    }

    [Fact]
    public void TestRollTooLong()
    {
      this.target.IsRollTooLong = true;
      Assert.True(this.target.IsRollTooLong);
      Assert.True(this.target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      this.anyChanged = false;
      this.target.IsRollTooLong = false;
      Assert.False(this.target.IsRollTooLong);
      Assert.True(this.target.VerifyRollSequence);
      Assert.False(this.anyChanged);
    }

    [Fact]
    public void TestRollTooShort()
    {
      this.target.IsRollTooShort = true;
      Assert.True(this.target.IsRollTooShort);
      Assert.True(this.target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      this.anyChanged = false;
      this.target.IsRollTooShort = false;
      Assert.False(this.target.IsRollTooShort);
      Assert.True(this.target.VerifyRollSequence);
      Assert.False(this.anyChanged);
    }

    [Fact]
    public void TestSystemDisabled()
    {
      this.target.IsSystemDisabled = true;
      Assert.True(this.target.IsSystemDisabled);
      Assert.True(this.target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      this.anyChanged = false;
      this.target.IsSystemDisabled = false;
      Assert.False(this.target.IsSystemDisabled);
      Assert.True(this.target.VerifyRollSequence);
      Assert.False(this.anyChanged);
    }

    [Fact]
    public void RollTooLongClearsRollTooShort()
    {
      this.target.IsRollTooShort = true;
      this.target.IsRollTooLong = true;
      Assert.True(this.target.IsRollTooLong);
      Assert.False(this.target.IsRollTooShort);
      Assert.Equal(1, this.anyChangesToTrue);
    }

    [Fact]
    public void RollTooShortClearsRollTooLong()
    {
      this.target.IsRollTooLong = true;
      this.target.IsRollTooShort = true;
      Assert.True(this.target.IsRollTooShort);
      Assert.False(this.target.IsRollTooLong);
      Assert.Equal(1, this.anyChangesToTrue);
    }

    [Fact]
    public void TestClearAll()
    {
      this.target.IsRollTooLong =
        this.target.IsRollTooShort =
        this.target.IsSystemDisabled = true;
      Assert.True(this.target.Any);
      Assert.Equal(1, this.anyChangesToTrue);

      this.target.ClearAll();
      Assert.False(this.target.Any);
      Assert.Equal(1, this.anyChangesToFalse);
    }
  }
}
