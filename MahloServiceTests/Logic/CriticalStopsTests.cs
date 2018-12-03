using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using MahloService.Logic;
using MahloService.Models;
using Xunit;

namespace MahloServiceTests.Logic
{
  public class CriticalStopsTests
  {
    private bool anyChanged;
    private int anyChangesToTrue;
    private int anyChangesToFalse;

    private CriticalStops<MahloModel> target;
    private readonly IDisposable subscription;

    public CriticalStopsTests()
    {
      this.target = new CriticalStops<MahloModel>();

      this.subscription =
      Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        h => ((INotifyPropertyChanged)this.target).PropertyChanged += h,
        h => ((INotifyPropertyChanged)this.target).PropertyChanged -= h)
        .Where(arg => arg.EventArgs.PropertyName == nameof(UserAttentions<MahloModel>.Any))
        .Subscribe(_ => this.anyChanged = (this.target.Any ? ++this.anyChangesToTrue : ++this.anyChangesToFalse) != 0);

      Assert.False(this.target.Any);
    }

    [Fact]
    public void TestMahloCommError()
    {
      this.target.IsMahloCommError = true;
      Assert.True(this.target.IsMahloCommError);
      Assert.True(this.target.Any);
      Assert.Equal(1, this.anyChangesToTrue);

      this.target.IsMahloCommError = false;
      Assert.False(this.target.IsMahloCommError);
      Assert.False(this.target.Any);
      Assert.Equal(1, this.anyChangesToFalse);
    }

    [Fact]
    public void TestMahloPLCError()
    {
      this.target.IsPlcCommError = true;
      Assert.True(this.target.IsPlcCommError);
      Assert.True(this.target.Any);
      Assert.Equal(1, this.anyChangesToTrue);

      this.target.IsPlcCommError = false;
      Assert.False(this.target.IsPlcCommError);
      Assert.False(this.target.Any);
      Assert.Equal(1, this.anyChangesToFalse);
    }

    [Fact]
    public void TestBoth()
    {
      this.target.IsPlcCommError = true;
      this.target.IsMahloCommError = true;
      Assert.True(this.target.IsPlcCommError);
      Assert.Equal(1, this.anyChangesToTrue);
      Assert.True(this.target.IsMahloCommError);

      this.anyChanged = false;
      this.target.IsPlcCommError = false;
      Assert.True(this.target.Any);
      Assert.False(this.anyChanged);
      this.target.IsMahloCommError = false;
      Assert.False(this.target.Any);
      Assert.Equal(1, this.anyChangesToFalse);
    }
  }
}
