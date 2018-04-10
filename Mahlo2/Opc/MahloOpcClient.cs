using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Models;
using System.Collections.Concurrent;
using System.Threading;
using OpcLabs.EasyOpc.DataAccess;
using OpcLabs.EasyOpc.DataAccess.OperationModel;

namespace Mahlo.Opc
{
  class MahloOpcClient : IMeterSrc, IBowAndSkewSrc, IPatternRepeatSrc, IWidthSrc
  {
    private const string MahloServerClass = "mahlo.10AOpcServer.1";
    private const string PlcServerClass = "Kepware.KEPServerEX.V6";
    //private const string MahloItemFormat = "nsu=mahlo.10AOpcServer.1;s={0}.{1}";
    //private const string PlcItemFormat = "nsu=KEPServerEX;ns=2;s={0}.{1}";
    private EasyDAClient opcClient;
    private Type priorExceptionType = null;
    private IMahloOpcSettings mahloSettings;
    private IPlcSettings seamSettings;
    private string seamResetTag;
    private SynchronizationContext synchronizationContext;

    public MahloOpcClient(EasyDAClient opcClient, IMahloOpcSettings mahloSettings, IPlcSettings seamSettings, SynchronizationContext synchronizationContext)
    {
      this.opcClient = opcClient;
      this.mahloSettings = mahloSettings;
      this.seamSettings = seamSettings;
      this.synchronizationContext = synchronizationContext;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string Recipe { get; set; }
    public double MetersCount { get; set; }
    public double MetersOffset { get; set; }
    public double Speed { get; set; }

    //public bool WidthOnOff { get; set; }
    //public int WidthStatus { get; set; }
    //public double WidthMeterStamp { get; set; }
    //public bool WidthValid { get; set; }
    //public double Width { get; set; }

    public bool OnOff { get; set; }
    public int Status { get; set; }
    public double MeterStamp { get; set; }
    public int ControllerState { get; set; }

    public bool SkewValid { get; set; }
    public double SkewInPercent { get; set; }
    public bool BowValid { get; set; }
    public double BowInPercent { get; set; }

    //public bool PrsOnOff { get; set; }
    //public int PrsStatus { get; set; }
    //public double PrsMeterStamp { get; set; }
    public bool Valid { get; set; }
    public double ValueInMeter { get; set; }
    //public int PrsControllerState { get; set; }

    public bool SeamDetected { get; set; }

    public void ResetMeterOffset()
    {
      throw new NotImplementedException();
    }

    public void ResetSeamDetector()
    {
      //UAWriteResult[] results = this.opcClient.WriteMultipleValues(new[]
      //{
      //  new UAWriteValueArguments(this.seamSettings.ServerUri, string.Format(PlcItemFormat, string.Empty, this.seamResetTag)),
      //});

      //var e = results[0];
      //if (e.Exception != null && e.Exception.GetType() != priorExceptionType)
      //{
      //  // TODO: Add logging
      //  //log.Error(e.Arguments.NodeDescriptor);
      //  //log.Error(e.Exception.GetType());
      //  //log.Error(e.ErrorMessage);
      //  this.priorExceptionType = e.Exception.GetType();
      //}
    }

    /// <summary>
    /// Subscribes to a list of OPC tags
    /// </summary>
    /// <param name="serverClass">The name of the OPC server.</param>
    /// <param name="channel">The channel with the OPC server.</param>
    /// <param name="items">The list of items to subscribe to.</param>
    private void Subscribe(string serverClass, string channel, IEnumerable<(string name, Action<object> action)> items)
    {
      var query = from item in items
                  let tag = $"{channel}.{item.name}"
                  select new EasyDAItemSubscriptionArguments(string.Empty, serverClass, tag, 250, ItemChangeCallback, item.action);

      var result = this.opcClient.SubscribeMultipleItems(query.ToArray());
    }

    private void MahloSubscribe(string channel, IEnumerable<(string name, Action<object> action)> items)
    {
      this.Subscribe(MahloServerClass, channel, items);
    }

    private void PlcSubscribe(string channel, IEnumerable<(string name, Action<object> action)> items)
    {
      this.Subscribe(PlcServerClass, channel, items);
    }

    public void Initialize<I>()
      where I : IMeterSrc
    {
      string mahloChannel;
      var mahloTags = new List<(string, Action<object>)>();
      mahloTags.AddRange(new(string, Action<object>)[]
      {
        ("Readings.Bridge.0.General.0.MeterCount", value => this.MetersCount = (double)value),
        ("Readings.Bridge.0.General.0.MeterOffset", value => this.MetersOffset = (double)value),
        ("Readings.Bridge.0.General.0.Speed", value => this.Speed = (double)value),
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

      if (typeof(I) == typeof(IBowAndSkewSrc))
      {
        mahloChannel = mahloSettings.BowAndSkewChannelName;
        mahloTags.AddRange(new(string, Action<object>)[]
        {
            ("Current.Version.0.KeyColumn", value => this.Recipe = (string)value),
            ("Current.Bridge.0.Calc.1.OnOff", value => this.OnOff = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.Status", value => this.Status = (int)value),
            ("Readings.Bridge.0.Calc.1.MeterStamp", value => this.MeterStamp = (double)value),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.SkewValid", value => this.SkewValid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.SkewInPercent", value => this.SkewInPercent = (double)value),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.BowValid", value => this.BowValid = (int)value == 1),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.BowInPercent", value => this.BowInPercent = (double)value),
            ("Readings.Bridge.0.Calc.1.CalcDistortion.0.Contr_State", value => this.ControllerState = (int)value),
        });
      }
      else if (typeof(I) == typeof(IPatternRepeatSrc))
      {
        mahloChannel = mahloSettings.PatternRepeatChannelName;
        mahloTags.AddRange(new(string, Action<object>)[]
        {
            ("Current.Bridge.0.Calc.2.OnOff", value => this.OnOff = (int)value == 1),
            ("Readings.Bridge.0.Calc.2.Status", value => this.Status = (int)value),
            ("Readings.Bridge.0.Calc.2.MeterStamp", value => this.MeterStamp = (double)value),
            ("Readings.Bridge.0.Calc.2.CalcLengthRepeat.0.Valid", value => this.Valid = (int)value == 1),
            ("Readings.Bridge.0.Calc.2.CalcLengthRepeat.0.ValueInMeter", value => this.ValueInMeter = (double)value),
            ("Readings.Bridge.0.Calc.2.CalcLengthRepeat.0.Contr_State", value => this.ControllerState = (int)value),
        });
      }
      else if (typeof(I) == typeof(IMeterSrc))
      {
        mahloChannel = mahloSettings.Mahlo2ChannelName;

        // Tags are already added
      }
      else
      {
          throw new ArgumentException();
      }

      this.MahloSubscribe(mahloChannel, mahloTags);
    }

    private void ItemChangeCallback(object sender, EasyDAItemChangedEventArgs e)
    {
      if (e.Exception != null && e.Exception.GetType() != priorExceptionType)
      {
        // TODO: Implement logging
        //log.Error(e.Arguments.NodeDescriptor);
        //log.Error(e.Exception.GetType());
        //log.Error(e.ErrorMessage);
        this.priorExceptionType = e.Exception.GetType();
      }

      if (e.Exception == null)
      {
        //this.synchronizationContext.Send(PostCallback, (e.Arguments.State, e.Vtq.Value));
        this.synchronizationContext.Post(new SendOrPostCallback((Action<object>)e.Arguments.State), e.Vtq.Value);

        //var setAction = (Action<object>)e.Arguments.State;
        //setAction(e.Vtq.Value);
        //log.DebugFormat("{0} = {1}", e.Arguments.NodeDescriptor, e.AttributeData.Value);
        this.priorExceptionType = null;
      }
    }
  }
}
