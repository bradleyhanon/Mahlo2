using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Repository;
using MahloService.Settings;
using Microsoft.AspNet.SignalR;
using OpcLabs.EasyOpc.DataAccess;
using Serilog;

namespace MahloService.Opc
{
  class OpcSrcSim<Model> : IMahloSrc, IBowAndSkewSrc, IPatternRepeatSrc, IOpcSim
  {
    private IUserAttentions<Model> userAttentions;
    private ICriticalStops<Model> criticalStops;
    private Type priorExceptionType = null;
    private IOpcSettings mahloSettings;
    //private IPlcSettings seamSettings;
    private IProgramState programState;
    private ILogger log;

    private SynchronizationContext synchronizationContext;

    List<IDisposable> disposables = new List<IDisposable>();

    public OpcSrcSim(
      IUserAttentions<Model> userAttentions,
      ICriticalStops<Model> criticalStops,
      IOpcSettings mahloSettings,
      //IPlcSettings seamSettings,
      IProgramState programState,
      SynchronizationContext synchronizationContext,
      ILogger logger)
    {
      this.userAttentions = userAttentions;
      this.criticalStops = criticalStops;
      this.mahloSettings = mahloSettings;
      //this.seamSettings = seamSettings;
      this.synchronizationContext = synchronizationContext;
      this.programState = programState;
      this.log = logger;

      this.disposables.AddRange(new IDisposable[]
      {
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.criticalStops).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.criticalStops).PropertyChanged -= h)
          .Where(args => args.EventArgs.PropertyName == nameof(CriticalStops<Model>.Any))
          .Subscribe(_ => this.SetCriticalAlarmIndicator(this.criticalStops.Any)),

        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.userAttentions).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.userAttentions).PropertyChanged -= h)
          .Where(args => args.EventArgs.PropertyName == nameof(UserAttentions<Model>.Any))
          .Subscribe(_ => this.SetStatusIndicator(this.userAttentions.Any)),
      });

      var state = programState.GetSubState(nameof(OpcClient<Model>), typeof(Model).Name);
      //this.Initialize();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public double FeetCounter { get; set; }

    public double FeetPerMinute { get; set; }

    public double MeasuredWidth { get; set; }

    public bool IsSeamDetected { get; set; }

    public string Recipe { get; set; }

    public bool IsAutoMode { get; set; }

    public double Bow { get; set; }

    public double Skew { get; set; }

    public double PatternRepeatLength { get; set; }

    public void ResetSeamDetector()
    {
      
    }

    public void SetAutoMode(bool value)
    {
      
    }

    public void SetCriticalAlarmIndicator(bool value)
    {
      
    }

    public void SetMiscellaneousIndicator(bool value)
    {
      
    }

    public void SetRecipe(string recipeName)
    {
      
    }

    public void SetStatusIndicator(bool value)
    {
      
    }

    public class SimHub : Hub
    {

    }
  }
}