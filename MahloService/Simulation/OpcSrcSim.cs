using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Settings;
using Microsoft.AspNet.SignalR;
using OpcLabs.EasyOpc.DataAccess;
using Serilog;

namespace MahloService.Simulation
{
  sealed class OpcSrcSim<Model> : IMahloSrc, IBowAndSkewSrc, IPatternRepeatSrc, IDisposable
  {
    private readonly ISewinQueue sewinQueue;
    private IUserAttentions<Model> userAttentions;
    private ICriticalStops<Model> criticalStops;
    private readonly IOpcSettings mahloSettings;
    //private IPlcSettings seamSettings;
    private readonly IProgramState programState;
    private readonly ILogger log;
    private readonly IServiceSettings settings;

    private readonly SynchronizationContext synchronizationContext;

    private GreigeRoll currentRoll;
    private int rollIndex;
    private int cutRollCount = 0;
    private double feetCounterAtRollStart;
    private readonly List<IDisposable> disposables = new List<IDisposable>();
    private IDisposable timer;
    private bool isCheckRollEndSeamNeeded;

    public OpcSrcSim(
      ISewinQueue sewinQueue,
      IUserAttentions<Model> userAttentions,
      ICriticalStops<Model> criticalStops,
      IOpcSettings mahloSettings,
      IProgramState programState,
      SynchronizationContext synchronizationContext,
      ILogger logger,
      IServiceSettings settings)
    {
      this.sewinQueue = sewinQueue;
      this.userAttentions = userAttentions;
      this.criticalStops = criticalStops;
      this.mahloSettings = mahloSettings;
      //this.seamSettings = seamSettings;
      this.synchronizationContext = synchronizationContext;
      this.programState = programState;
      this.log = logger;
      this.settings = settings;

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

      var state = programState.GetSubState(nameof(OpcSrcSim<Model>), typeof(Model).Name);
      this.cutRollCount = state.Get<int?>(nameof(cutRollCount)) ?? 0;
      this.rollIndex = state.Get<int?>(nameof(rollIndex)) ?? 0;
      this.FeetCounter = state.Get<double?>(nameof(FeetCounter)) ?? this.FeetCounter;
      this.feetCounterAtRollStart = state.Get<double?>(nameof(feetCounterAtRollStart)) ?? this.feetCounterAtRollStart;

      this.isCheckRollEndSeamNeeded = this.sewinQueue.Rolls.FirstOrDefault()?.IsCheckRoll ?? false;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public double FeetCounter { get; set; } = -1.0;

    public double FeetPerMinute { get; set; }

    public double MeasuredWidth { get; set; }

    public bool IsSeamDetected { get; set; }

    public string Recipe { get; set; }

    public bool IsAutoMode { get; set; }

    public double Bow { get; set; }

    public double Skew { get; set; }

    public double PatternRepeatLength { get; set; }

    public bool IsDoffDetected { get; set; }

    public void Dispose()
    {
      if (this.currentRoll != null)
      {
        this.rollIndex = this.sewinQueue.Rolls.IndexOf(this.currentRoll);
      }

      var state = this.programState.GetSubState(nameof(OpcSrcSim<Model>));
      state.Set(typeof(Model).Name, new
      {
        this.cutRollCount,
        this.rollIndex,
        this.FeetCounter,
        this.feetCounterAtRollStart,
      });
    }

    public void AcknowledgeSeamDetect()
    {
      this.IsSeamDetected = false;
    }

    public void AcknowledgeDoffDetect()
    {
      this.IsDoffDetected = false;
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

    public void Start(double feetPerMinute)
    {
      this.FeetPerMinute = feetPerMinute;
      if (this.rollIndex < this.sewinQueue.Rolls.Count)
      {
        this.currentRoll = this.sewinQueue.Rolls[this.rollIndex];
        this.isCheckRollEndSeamNeeded = this.currentRoll.IsCheckRoll;
        this.timer = Observable.Interval(TimeSpan.FromMinutes(1 / FeetPerMinute))
          .ObserveOn(SynchronizationContext.Current)
          .Subscribe(timer =>
          {
            //Console.WriteLine($"{typeof(Model).Name}: FeetCounter={this.FeetCounter}");
            double measuredLength = this.FeetCounter - this.feetCounterAtRollStart;
            var percentComplete = measuredLength / this.currentRoll.RollLength;
            this.Bow = 2.0 * percentComplete;
            this.Skew = 2.0 * percentComplete;
            this.PatternRepeatLength = this.currentRoll.PatternRepeatLength * ((1.045 - 0.955) * percentComplete + 0.955);

            if (this.isCheckRollEndSeamNeeded && this.currentRoll.IsCheckRoll)
            {
              //Console.WriteLine($"Left={currentRoll.RollLength - measuredLength}, Target={this.settings.MaxEndCheckRollPieceLength}");
              if (this.currentRoll.RollLength - measuredLength < this.settings.MaxEndCheckRollPieceLength)
              {
                this.IsSeamDetected = true;
                this.isCheckRollEndSeamNeeded = false;
                Console.WriteLine($"CheckRollSeam Seen at {measuredLength}");
              }
            }

            if (this.FeetCounter - this.feetCounterAtRollStart >= this.currentRoll.RollLength)
            {
              cutRollCount = 1;
              this.IsSeamDetected = true;
              this.IsDoffDetected = true;
              this.feetCounterAtRollStart = this.FeetCounter;
              this.rollIndex = this.sewinQueue.Rolls.IndexOf(this.currentRoll);
              if (++this.rollIndex >= this.sewinQueue.Rolls.Count)
              {
                this.Stop();
              }
              else
              {
                this.currentRoll = this.sewinQueue.Rolls[this.rollIndex];
                this.isCheckRollEndSeamNeeded = this.currentRoll.IsCheckRoll;
              }
            }
            else if ((int)measuredLength == cutRollCount * (this.currentRoll.RollLength / 4))
            {
              cutRollCount++;
              this.IsDoffDetected = true;
            }

            this.FeetCounter += 1.0;
          });
      }
    }
  
    public void Stop()
    {
      this.timer?.Dispose();
      this.timer = null;
    }
  }
}