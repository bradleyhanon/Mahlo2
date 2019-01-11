using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;
using MahloService.Utilities;
using OpcLabs.EasyOpc.DataAccess;
using OpcLabs.EasyOpc.DataAccess.OperationModel;
using PropertyChanged;
using Serilog;

namespace MahloService.Opc
{
  [AddINotifyPropertyChangedInterface]
  internal class OpcClient<Model> : IMahloSrc, IBowAndSkewSrc, IPatternRepeatSrc
  {
    private const string MahloServerClass = "mahlo.10AOpcServer.1";
    private const string PlcServerClass = "Kepware.KEPServerEX.V6";
    //private const string MeterCountTag = "Readings.Bridge.0.General.0.MeterCount";

    //private const string MahloItemFormat = "nsu=mahlo.10AOpcServer.1;s={0}.{1}";
    //private const string PlcItemFormat = "nsu=KEPServerEX;ns=2;s={0}.{1}";
    private string mahloChannel;

    private readonly IEasyDAClient opcClient;
    private IUserAttentions<Model> userAttentions;
    private ICriticalStops<Model> criticalStops;
    private Type priorExceptionType = null;
    private readonly IOpcSettings mahloSettings;
    private readonly ILogger log;

    private readonly SynchronizationContext synchronizationContext;
    private string seamAckTag;

    private readonly IDisposable criticalAlarmsSubscription;
    private readonly IDisposable userAttentionsSubscription;

    public OpcClient(
      IEasyDAClient opcClient,
      IUserAttentions<Model> userAttentions,
      ICriticalStops<Model> criticalStops,
      IOpcSettings mahloSettings,
      //IPlcSettings seamSettings,
      SynchronizationContext synchronizationContext,
      ILogger logger)
    {
      this.opcClient = opcClient;
      this.userAttentions = userAttentions;
      this.criticalStops = criticalStops;
      this.mahloSettings = mahloSettings;
      //this.seamSettings = seamSettings;
      this.synchronizationContext = synchronizationContext;
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

      this.Initialize();
    }

    public double FeetCounter { get; set; }
    public double FeetPerMinute { get; set; }
    public double MeasuredWidth { get; set; }
    public double BowInInches { get; set; }
    public double SkewInInches { get; set; }
    public double PatternRepeatLength { get; set; }
    public bool IsSeamDetected { get; set; }
    public bool IsDoffDetected { get; set; }

    public bool IsAutoMode { get; set; }
    public string Recipe { get; set; }

    public void AcknowledgeSeamDetect()
    {
      Task.Run(() =>
      {
        this.opcClient.WriteItemValue(string.Empty, PlcServerClass, this.seamAckTag, 1);
        this.opcClient.WriteItemValue(string.Empty, PlcServerClass, this.seamAckTag, 0);
      }).NoWait();
    }

    public void AcknowledgeDoffDetect()
    {
      this.opcClient.WriteItemValue(string.Empty, PlcServerClass, "MahloSeam.MahloPLC.MahloDoffAck", 1);
      this.opcClient.WriteItemValue(string.Empty, PlcServerClass, "MahloSeam.MahloPLC.MahloDoffAck", 0);
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
      this.Subscribe(MahloServerClass, channel, items, this.MahloItemChangedCallback);
    }

    private void PlcSubscribe(string channel, IEnumerable<(string name, Action<object> action)> items)
    {
      this.Subscribe(PlcServerClass, channel, items, this.PlcItemChangedCallback);
    }

    public void Initialize()
    {
      int seamDetectorId;
      var plcTags = new List<(string, Action<object>)>();
      var mahloTags = new List<(string, Action<object>)>();
      mahloTags.AddRange(new (string, Action<object>)[]
      {
        ("Current.Version.0.KeyColumn", value => this.Recipe = (string)value),
        ("Readings.Bridge.0.General.0.MeterCount", value =>  this.FeetCounter = ServiceExtensions.MetersToFeet((double)value)),
        //("Readings.Bridge.0.General.0.MeterOffset", value => this.MetersOffset = (double)value),
        ("Readings.Bridge.0.General.0.Speed", value => this.FeetPerMinute = ServiceExtensions.MetersToFeet((double)value) * 60),
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

      if (typeof(Model) == typeof(BowAndSkewModel))
      {
        seamDetectorId = 2;
        this.mahloChannel = this.mahloSettings.BowAndSkewChannelName;
        mahloTags.AddRange(new (string, Action<object>)[]
        {
            ("Current.Bridge.0.Calc.1.OnOff", value => this.IsAutoMode = (int)value != 0),
            //("Readings.Bridge.0.Calc.1.Status", value => this.Status = (int)value),
            //("Readings.Bridge.0.Calc.1.MeterStamp", value => this.MeterStamp = (double)value),
            //("Readings.Bridge.0.Calc.1.CalcDistortion.0.SkewValid", value => this.SkewValid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.SkewInPercent", value => this.SkewInInches = ServiceExtensions.MetersToFeet((double)value * this.MeasuredWidth)),
            //("Readings.Bridge.0.Calc.1.CalcDistortion.0.BowValid", value => this.BowValid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.BowInPercent", value => this.BowInInches = ServiceExtensions.MetersToFeet((double)value * this.MeasuredWidth)),
            //("Readings.Bridge.0.Calc.1.CalcDistortion.0.Contr_State", value => this.ControllerState = (int)value),
            ("Readings.Bridge.0.Calc.0.CalcWidth.0.ValueInMeter", value => this.MeasuredWidth = ServiceExtensions.MetersToFeet((double)value) * 12),
        });
      }
      else if (typeof(Model) == typeof(PatternRepeatModel))
      {
        plcTags.Add(($"MahloPLC.MahloDoff", value => this.IsDoffDetected = (bool)value));

        seamDetectorId = 3;
        this.mahloChannel = this.mahloSettings.PatternRepeatChannelName;
        mahloTags.AddRange(new (string, Action<object>)[]
        {
            ("Current.Bridge.0.Calc.1.OnOff", value => this.IsAutoMode = (int)value != 0),
            //("Readings.Bridge.0.Calc.1.Status", value => this.Status = (int)value),
            //("Readings.Bridge.0.Calc.1.MeterStamp", value => this.MeterStamp = (double)value),
            //("Readings.Bridge.0.Calc.1.CalcLengthRepeat.0.Valid", value => this.Valid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcLengthRepeat.0.ValueInMeter", value => this.PatternRepeatLength = ServiceExtensions.MetersToFeet((double)value) * 12),
            //("Readings.Bridge.0.Calc.1.CalcLengthRepeat.0.Contr_State", value => this.ControllerState = (int)value),
        });
      }
      else if (typeof(Model) == typeof(MahloModel))
      {
        seamDetectorId = 1;
        this.mahloChannel = this.mahloSettings.Mahlo2ChannelName;

        mahloTags.Clear();
        mahloTags.AddRange(new (string, Action<object>)[]
        {
          //("Current.Version.0.KeyColumn", value => this.Recipe = (string)value),
          ("Readings.Metercounter.0.Value", value => this.FeetCounter = ServiceExtensions.MetersToFeet((double)value)),
          //("Readings.Bridge.0.General.0.MeterOffset", value => this.MetersOffset = (double)value),
          //("Readings.Bridge.0.General.0.Speed", value => { this.speedSubject.OnNext((double)value); }),
        });
      }
      else
      {
        throw new ArgumentException("Invalid model type");
      }

      this.seamAckTag = $"MahloSeam.MahloPLC.Mahlo{seamDetectorId}SeamAck";
      plcTags.Add(($"MahloPLC.Mahlo{seamDetectorId}SeamDetected", value => this.IsSeamDetected = (bool)value));
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
        if (e.Exception.GetType() != this.priorExceptionType)
        {
          this.log.Debug(e.Exception, e.Arguments.ItemDescriptor.ItemId);
          // TODO: Implement logging
          //log.Error(e.Arguments.NodeDescriptor);
          //log.Error(e.Exception.GetType());
          //log.Error(e.ErrorMessage);
          this.priorExceptionType = e.Exception.GetType();
        }
        else
        {
          this.log.Debug(e.Arguments.ItemDescriptor.ItemId);
        }
      }

      if (e.Exception == null && e.Vtq.HasValue)
      {
        //this.synchronizationContext.Send(PostCallback, (e.Arguments.State, e.Vtq.Value));
        TaskUtilities.RunOnMainThreadAsync(() => ((Action<object>)e.Arguments.State)(e.Vtq.Value)).NoWait();

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

    public void SetRecipeFromPatternLength(double targetPatternRepeatLength)
    {
      const int Small = 1;
      const int Middle = 2;
      const int Large = 3;

      int level =
        targetPatternRepeatLength <= 10.0 ? Small :
        targetPatternRepeatLength <= 25.0 ? Middle : Large;

      this.opcClient.WriteItemValue(string.Empty, MahloServerClass, $"{this.mahloChannel}.Current.Bridge.0.Algo.2.AlgoVertPattern.0.StartLevel", level);
    }
  }
}
