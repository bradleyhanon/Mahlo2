using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using Mahlo.Opc;
using NSubstitute;
using Xunit;

namespace Mahlo2Tests.Logic
{
  public class UserAttententionsTests : IDisposable
  {
    //VerifyRollSequence = 1,
    //  RollTooLong = 2,
    //  RollTooShort = 4,
    //  SystemDisabled = 8,
    //  All = VerifyRollSequence | RollTooLong | RollTooShort | SystemDisabled,

    bool anyChanged;
    int anyChangesToTrue;
    int anyChangesToFalse;
    UserAttentions<MahloRoll> target;
    IDisposable subscription;

    public UserAttententionsTests()
    {
      target = new UserAttentions<MahloRoll>();

      this.subscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.target).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.target).PropertyChanged -= h)
          .Where(arg => arg.EventArgs.PropertyName == nameof(UserAttentions<MahloRoll>.Any))
          .Subscribe(_ => anyChanged = (this.target.Any ? ++this.anyChangesToTrue : ++this.anyChangesToFalse) != 0);

      Assert.False(target.Any);
    }

    public void Dispose()
    {
      this.subscription.Dispose();
    }

    [Fact]
    public void TestVerifyRollSequence()
    {
      target.VerifyRollSequence = true;
      Assert.True(target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      target.VerifyRollSequence = false;
      Assert.False(target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToFalse);
    }

    [Fact]
    public void TestRollTooLong()
    {
      target.IsRollTooLong = true;
      Assert.True(target.IsRollTooLong);
      Assert.True(target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      this.anyChanged = false;
      target.IsRollTooLong = false;
      Assert.False(target.IsRollTooLong);
      Assert.True(target.VerifyRollSequence);
      Assert.False(anyChanged);
    }

    [Fact]
    public void TestRollTooShort()
    {
      target.IsRollTooShort = true;
      Assert.True(target.IsRollTooShort);
      Assert.True(target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      this.anyChanged = false;
      target.IsRollTooShort = false;
      Assert.False(target.IsRollTooShort);
      Assert.True(target.VerifyRollSequence);
      Assert.False(this.anyChanged);
    }

    [Fact]
    public void TestSystemDisabled()
    {
      target.IsSystemDisabled = true;
      Assert.True(target.IsSystemDisabled);
      Assert.True(target.VerifyRollSequence);
      Assert.Equal(1, this.anyChangesToTrue);

      this.anyChanged = false;
      target.IsSystemDisabled = false;
      Assert.False(target.IsSystemDisabled);
      Assert.True(target.VerifyRollSequence);
      Assert.False(this.anyChanged);
    }

    [Fact]
    public void RollTooLongClearsRollTooShort()
    {
      target.IsRollTooShort = true;
      target.IsRollTooLong = true;
      Assert.True(target.IsRollTooLong);
      Assert.False(target.IsRollTooShort);
      Assert.Equal(1, this.anyChangesToTrue);
    }

    [Fact]
    public void RollTooShortClearsRollTooLong()
    {
      target.IsRollTooLong = true;
      target.IsRollTooShort = true;
      Assert.True(target.IsRollTooShort);
      Assert.False(target.IsRollTooLong);
      Assert.Equal(1, this.anyChangesToTrue);
    }

    [Fact]
    public void TestClearAll()
    {
      target.IsRollTooLong =
        target.IsRollTooShort =
        target.IsSystemDisabled = true;
      Assert.True(target.Any);
      Assert.Equal(1, this.anyChangesToTrue);

      target.ClearAll();
      Assert.False(target.Any);
      Assert.Equal(1, this.anyChangesToFalse);
    }
  }
}
