﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Models;
using System.Collections.Concurrent;
using System.Threading;
using OpcLabs.EasyOpc.DataAccess;
using OpcLabs.EasyOpc.DataAccess.OperationModel;
using OpcLabs.EasyOpc.DataAccess.Generic;
using MahloService.Repository;
using MahloService.Logic;
using Serilog;
using PropertyChanged;

namespace MahloService.Opc
{
  [AddINotifyPropertyChangedInterface]
  sealed class MahloOpcClient<Model> : IMahloSrc, IBowAndSkewSrc, IPatternRepeatSrc, IDisposable
  {
    private const string MahloServerClass = "mahlo.10AOpcServer.1";
    private const string PlcServerClass = "Kepware.KEPServerEX.V6";
    private const string MeterCountTag = "Readings.Bridge.0.General.0.MeterCount";
    //private const string MahloItemFormat = "nsu=mahlo.10AOpcServer.1;s={0}.{1}";
    //private const string PlcItemFormat = "nsu=KEPServerEX;ns=2;s={0}.{1}";
    string mahloChannel;

    private EasyDAClient opcClient;
    private IUserAttentions<Model> userAttentions;
    private ICriticalStops<Model> criticalStops;
    private Type priorExceptionType = null;
    private IOpcSettings mahloSettings;
    //private IPlcSettings seamSettings;
    private IProgramState programState;
    private ILogger log;

    private SynchronizationContext synchronizationContext;
    private string seamAckTag;
    private string seamDetectedTag;
    private double meterOffset;

    private Subject<double> meterCountSubject = new Subject<double>();
    private Subject<double> speedSubject = new Subject<double>();
    private Subject<bool> seamDetectedSubject = new Subject<bool>();

    private Subject<double> bowSubject = new Subject<double>();
    private Subject<double> skewSubject = new Subject<double>();
    private Subject<double> patterRepeatSubject = new Subject<double>();
    private Subject<double> widthSubject = new Subject<double>();

    private IDisposable criticalAlarmsSubscription;
    private IDisposable userAttentionsSubscription;

    public MahloOpcClient(
      EasyDAClient opcClient, 
      IUserAttentions<Model> userAttentions,
      ICriticalStops<Model> criticalStops, 
      IOpcSettings mahloSettings, 
      //IPlcSettings seamSettings,
      IProgramState programState,
      SynchronizationContext synchronizationContext,
      ILogger logger)
    {
      this.opcClient = opcClient;
      this.userAttentions = userAttentions;
      this.criticalStops = criticalStops;
      this.mahloSettings = mahloSettings;
      //this.seamSettings = seamSettings;
      this.synchronizationContext = synchronizationContext;
      this.programState = programState;
      this.log = logger;

      this.criticalAlarmsSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.criticalStops).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.criticalStops).PropertyChanged -= h)
          .Where(args => args.EventArgs.PropertyName == nameof(CriticalStops<Model>.Any))
          .Subscribe(_ => this.SetCriticalAlarmIndicator(this.criticalStops.Any));

      this.userAttentionsSubscription =
        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.userAttentions).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.userAttentions).PropertyChanged -= h)
          .Where(args => args.EventArgs.PropertyName == nameof(UserAttentions<Model>.Any))
          .Subscribe(_ => this.SetStatusIndicator(this.userAttentions.Any));

      var state = programState.GetSubState(nameof(MahloOpcClient<Model>), typeof(Model).Name);
      this.meterOffset = state.Get<double?>(nameof(meterOffset)) ?? 0.0;
      this.Initialize();
    }

    public bool IsAutoMode { get; set; }
    public string Recipe { get; set; }
    public double MetersCount { get; set; }
    //public double MetersOffset { get; set; }
    //public double Speed { get; set; }

    public IObservable<int> FeetCounter => this.meterCountSubject
      .Select(meters => (int)Extensions.MetersToFeet(meters))
      .DistinctUntilChanged();

    public IObservable<int> FeetPerMinute => this.speedSubject
      .Select(speed => (int)(Extensions.MetersToFeet(speed) * 60))
      .DistinctUntilChanged();
   
    public IObservable<bool> SeamDetected => this.seamDetectedSubject;

    public IObservable<double> BowChanged => this.bowSubject;

    public IObservable<double> SkewChanged => this.skewSubject;

    public IObservable<double> PatternRepeatChanged => this.patterRepeatSubject
      .Select(value => Extensions.MetersToFeet(value) * 12);

    public IObservable<double> WidthChanged => this.widthSubject
      .Select(value => Extensions.MetersToFeet(value))
      .DistinctUntilChanged();

    //public bool WidthOnOff { get; set; }
    //public int WidthStatus { get; set; }
    //public double WidthMeterStamp { get; set; }
    //public bool WidthValid { get; set; }
    //public double Width { get; set; }

    //public bool OnOff { get; set; }
    //public int Status { get; set; }
    //public double MeterStamp { get; set; }
    //public int ControllerState { get; set; }

    //public bool SkewValid { get; set; }
    //public double SkewInPercent { get; set; }
    //public bool BowValid { get; set; }
    //public double BowInPercent { get; set; }

    //public bool PrsOnOff { get; set; }
    //public int PrsStatus { get; set; }
    //public double PrsMeterStamp { get; set; }
    //public bool Valid { get; set; }
    //public double ValueInMeter { get; set; }

    //public int PrsControllerState { get; set; }

    public void Dispose()
    {
      var obj = this.programState.GetSubState(nameof(MahloOpcClient<Model>), typeof(Model).Name);
      obj.Set(nameof(meterOffset), this.meterOffset);
    }

    public void ResetMeterOffset()
    {
      this.meterOffset = this.MetersCount;
    }

    public void ResetSeamDetector()
    {
      Task.Run(() =>
      {
        this.opcClient.WriteItemValue(string.Empty, PlcServerClass, this.seamAckTag, 1);
        this.opcClient.WriteItemValue(string.Empty, PlcServerClass, this.seamAckTag, 0);
      });
    }

    /// <summary>
    /// Subscribes to a list of OPC tags
    /// </summary>
    /// <param name="serverClass">The name of the OPC server.</param>
    /// <param name="channel">The channel with the OPC server.</param>
    /// <param name="items">The list of items to subscribe to.</param>
    private void Subscribe(string serverClass, string channel, IEnumerable<(string name, Action<object> action)> items, EasyDAItemChangedEventHandler callback)
    {
      var query = from item in items
                  let tag = $"{channel}.{item.name}"
                  select new EasyDAItemSubscriptionArguments(string.Empty, serverClass, tag, 250, callback, item.action);

      var result = this.opcClient.SubscribeMultipleItems(query.ToArray());
    }

    private void MahloSubscribe(string channel, IEnumerable<(string name, Action<object> action)> items)
    {
      this.Subscribe(MahloServerClass, channel, items, MahloItemChangedCallback);
    }

    private void PlcSubscribe(string channel, IEnumerable<(string name, Action<object> action)> items)
    {
      this.Subscribe(PlcServerClass, channel, items, PlcItemChangedCallback);
    }

    public void Initialize()
    {
      int seamDetectorId;
      var mahloTags = new List<(string, Action<object>)>();
      mahloTags.AddRange(new(string, Action<object>)[]
      {
        ("Current.Version.0.KeyColumn", value => this.Recipe = (string)value),
        ("Readings.Bridge.0.General.0.MeterCount", value => {this.MetersCount = (double)value; this.meterCountSubject.OnNext((double)value - this.meterOffset); }),
        //("Readings.Bridge.0.General.0.MeterOffset", value => this.MetersOffset = (double)value),
        ("Readings.Bridge.0.General.0.Speed", value => { this.speedSubject.OnNext((double)value); }),
      });

      //case IWidthSrc widthSrc:
      //  mahloTags.AddRange(new(string, Action<object>)[]
      //  {
      //    ("Current.Bridge.0.Calc.0.OnOff", value => this.OnOff = (int)value == 1),
      //    ("Readings.Bridge.0.Calc.0.Status", value => this.Status = (int)value),
      //    ("Readings.Bridge.0.Calc.0.MeterStamp", value => this.MeterStamp = (double)value),
      //    ("Readings.Bridge.0.Calc.0.CalcWidth.0.Valid", value => this.Valid = (int)value == 1),
      //    ("Readings.Bridge.0.Calc.0.CalcWidth.0.ValueInMeter", value => this.ValueInMeter = (double)value),
      //  });
      //  break;

      if (typeof(Model) == typeof(BowAndSkewRoll))
      {
        seamDetectorId = 2;
        this.mahloChannel = mahloSettings.BowAndSkewChannelName;
        mahloTags.AddRange(new(string, Action<object>)[]
        {
            ("Current.Bridge.0.Calc.1.OnOff", value => this.IsAutoMode = (int)value != 0),
            //("Readings.Bridge.0.Calc.1.Status", value => this.Status = (int)value),
            //("Readings.Bridge.0.Calc.1.MeterStamp", value => this.MeterStamp = (double)value),
            //("Readings.Bridge.0.Calc.1.CalcDistortion.0.SkewValid", value => this.SkewValid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.SkewInPercent", value => this.skewSubject.OnNext((double)value)),
            //("Readings.Bridge.0.Calc.1.CalcDistortion.0.BowValid", value => this.BowValid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.BowInPercent", value => this.bowSubject.OnNext((double)value)),
            //("Readings.Bridge.0.Calc.1.CalcDistortion.0.Contr_State", value => this.ControllerState = (int)value),
            ("Readings.Bridge.0.Calc.0.CalcWidth.0.ValueInMeter", value => this.widthSubject.OnNext((double)value)),
        });
      }
      else if (typeof(Model) == typeof(PatternRepeatRoll))
      {
        seamDetectorId = 3;
        this.mahloChannel = mahloSettings.PatternRepeatChannelName;
        mahloTags.AddRange(new(string, Action<object>)[]
        {
            ("Current.Bridge.0.Calc.1.OnOff", value => this.IsAutoMode = (int)value != 0),
            //("Readings.Bridge.0.Calc.1.Status", value => this.Status = (int)value),
            //("Readings.Bridge.0.Calc.1.MeterStamp", value => this.MeterStamp = (double)value),
            //("Readings.Bridge.0.Calc.1.CalcLengthRepeat.0.Valid", value => this.Valid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcLengthRepeat.0.ValueInMeter", value => this.patterRepeatSubject.OnNext((double)value)),
            //("Readings.Bridge.0.Calc.1.CalcLengthRepeat.0.Contr_State", value => this.ControllerState = (int)value),
        });
      }
      else if (typeof(Model) == typeof(MahloRoll))
      {
        seamDetectorId = 1;
        this.mahloChannel = mahloSettings.Mahlo2ChannelName;

        mahloTags.Clear();
        mahloTags.AddRange(new(string, Action<object>)[]
        {
          //("Current.Version.0.KeyColumn", value => this.Recipe = (string)value),
          ("Readings.Metercounter.0.Value", value => {this.MetersCount = (double)value; this.meterCountSubject.OnNext((double)value - this.meterOffset); }),
          //("Readings.Bridge.0.General.0.MeterOffset", value => this.MetersOffset = (double)value),
          //("Readings.Bridge.0.General.0.Speed", value => { this.speedSubject.OnNext((double)value); }),
        });
      }
      else
      {
          throw new ArgumentException();
      }

      this.seamAckTag = $"MahloSeam.MahloPLC.Mahlo{seamDetectorId}SeamAck";
      this.seamDetectedTag = $"MahloSeam.MahloPLC.Mahlo{seamDetectorId}SeamDetected";
      var plcTags = new List<(string, Action<object>)>()
      {
        ($"MahloPLC.Mahlo{seamDetectorId}SeamDetected", value => this.seamDetectedSubject.OnNext((bool)value)),
      };

      this.PlcSubscribe("MahloSeam", plcTags);
      this.MahloSubscribe(this.mahloChannel, mahloTags);
    }

    private void MahloItemChangedCallback(object sender, EasyDAItemChangedEventArgs e)
    {
      this.criticalStops.IsMahloCommError = e.Exception != null || !e.Vtq.HasValue;
      this.ItemChangeCallback(sender, e);
    }

    private void PlcItemChangedCallback(object sender, EasyDAItemChangedEventArgs e)
    {
      this.criticalStops.IsPlcCommError = e.Exception != null || !e.Vtq.HasValue;
      this.ItemChangeCallback(sender, e);
    }

    private void ItemChangeCallback(object sender, EasyDAItemChangedEventArgs e)
    {
      if (e.Exception != null)
      {
        if (e.Exception.GetType() != priorExceptionType)
        {
          log.Debug(e.Exception, e.Arguments.ItemDescriptor.ItemId);
          // TODO: Implement logging
          //log.Error(e.Arguments.NodeDescriptor);
          //log.Error(e.Exception.GetType());
          //log.Error(e.ErrorMessage);
          this.priorExceptionType = e.Exception.GetType();
        }
        else
        {
          log.Debug(e.Arguments.ItemDescriptor.ItemId);
        }
      }

      if (e.Exception == null && e.Vtq.HasValue)
      {
        //this.synchronizationContext.Send(PostCallback, (e.Arguments.State, e.Vtq.Value));
        this.synchronizationContext.Post(new SendOrPostCallback((Action<object>)e.Arguments.State), e.Vtq.Value);

        //var setAction = (Action<object>)e.Arguments.State;
        //setAction(e.Vtq.Value);
        //log.DebugFormat("{0} = {1}", e.Arguments.NodeDescriptor, e.AttributeData.Value);
        this.priorExceptionType = null;
      }
    }

    public void SetStatusIndicator(bool value)
    {
      //throw new NotImplementedException();
    }

    public void SetCriticalAlarmIndicator(bool value)
    {
      //throw new NotImplementedException();
    }

    public void SetMiscellaneousIndicator(bool value)
    {
      //throw new NotImplementedException();
    }

    public void SetAutoMode(bool value)
    {
      if (value != this.IsAutoMode)
      {
        var datvq = new DAVtq(value ? 1 : 0);
        //this.opcClient.WriteItem(string.Empty, MahloServerClass, $"{this.mahloChannel}.Current.Bridge.0.Calc.1.OnOff", datvq);
      }
    }

    public void SetRecipe(string recipeName)
    {
      if (recipeName != this.Recipe)
      {
        //this.opcClient.WriteItemValue(string.Empty, MahloServerClass, $"{this.mahloChannel}.ApplyRecipe", recipeName);
      }
    }
  }
}