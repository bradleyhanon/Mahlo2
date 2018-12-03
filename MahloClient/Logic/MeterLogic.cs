using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MahloClient.Ipc;
using MahloService;
using MahloService.Logic;
using MahloService.Models;
using MahloService.Settings;
using MahloService.Utilities;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json.Linq;
using PropertyChanged;

namespace MahloClient.Logic
{
  internal abstract class MeterLogic<Model> : IMeterLogic, IStatusBarInfo, IDisposable, INotifyPropertyChanged
  {
    private IMahloIpcClient ipcClient;
    private ISewinQueue sewinQueue;
    private readonly IServiceSettings serviceSettings;
    private int currentRollIndex = -1;
    private GreigeRoll currentRoll = new GreigeRoll();

    private readonly IDisposable sewinQueueChangedSubscription;
    private readonly IDisposable updateMeterLogicSubscription;
    private readonly IDisposable connectionStateSubscription;
    private readonly IDisposable userAttentionsSubscription;
    private readonly IDisposable criticalStopsSubscription;

    public MeterLogic(IMahloIpcClient ipcClient, ISewinQueue sewinQueue, IServiceSettings serviceSettings)
    {
      this.ipcClient = ipcClient;
      this.sewinQueue = sewinQueue;
      this.serviceSettings = serviceSettings;

      this.sewinQueueChangedSubscription = Observable
        .FromEventPattern<EventHandler, EventArgs>(
          h => this.sewinQueue.Changed += h,
          h => this.sewinQueue.Changed -= h)
        .Subscribe(args =>
        {
          bool indexInRange = this.CurrentRollIndex >= 0 && this.currentRollIndex < this.sewinQueue.Rolls.Count;
          this.CurrentRoll = indexInRange ? this.sewinQueue.Rolls[this.CurrentRollIndex] : new GreigeRoll();
        });

      this.updateMeterLogicSubscription = Observable
        .FromEvent<Action<(string, JObject)>, (string name, JObject jObject)>(
          h => this.ipcClient.MeterLogicUpdated += h,
          h => this.ipcClient.MeterLogicUpdated -= h)
        .Where(tuple => tuple.name == this.InterfaceName)
        .Subscribe(tuple =>
        {
          tuple.jObject.Populate(this);
          this.RefreshStatusDisplay();
        });

      this.connectionStateSubscription = Observable
        .FromEvent<Action<string>, string>(
          h => this.ipcClient.IpcStatusMessageChanged += h,
          h => this.ipcClient.IpcStatusMessageChanged -= h)
        .Subscribe(message => this.ConnectionStatusMessage = message);

      this.userAttentionsSubscription = Observable
        .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.UserAttentions).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.UserAttentions).PropertyChanged -= h)
        .Subscribe(args => this.OnPropertyChanged(nameof(this.UserAttentions)));

      this.criticalStopsSubscription = Observable
        .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
          h => ((INotifyPropertyChanged)this.CriticalStops).PropertyChanged += h,
          h => ((INotifyPropertyChanged)this.CriticalStops).PropertyChanged += h)
        .Subscribe(args => this.OnPropertyChanged(nameof(this.CriticalStops)));
    }

    public event PropertyChangedEventHandler PropertyChanged;

#pragma warning disable 67
    // These events are not used in the unit tests but no by the application.
    public event Action<GreigeRoll> RollStarted;
    public event Action<GreigeRoll> RollFinished;
#pragma warning restore 67

    public abstract string InterfaceName { get; }

    public string ConnectionStatusMessage { get; set; }

    public bool IsConnectionEstablished => this.ipcClient.State == ConnectionState.Connected;

    public bool IsChanged { get; set; }

    public GreigeRoll CurrentRoll
    {
      get => this.currentRoll;
      set
      {
        this.currentRoll = value;
        this.CurrentRollType = CommonMethods.DetermineRollType(this.sewinQueue.Rolls, this.currentRoll);
        this.NextRoll = this.sewinQueue.Rolls.SkipWhile(roll => roll != this.CurrentRoll).Skip(1).FirstOrDefault();
        this.NextRollType = this.NextRoll == null ? (RollTypeEnum?)null : CommonMethods.DetermineRollType(this.sewinQueue.Rolls, this.NextRoll);
      }
    }

    public GreigeRoll NextRoll { get; set; } = new GreigeRoll();

    public int CurrentRollIndex
    {
      get => this.currentRollIndex;
      set
      {
        this.currentRollIndex = value;
        bool isIndexInRange = value >= 0 && value < this.sewinQueue.Rolls.Count;
        this.CurrentRoll = isIndexInRange ? this.sewinQueue.Rolls[value] : new GreigeRoll();
      }
    }

    public RollTypeEnum CurrentRollType { get; set; }
    public RollTypeEnum? NextRollType { get; set; }

    public bool IsMappingNow { get; set; }

    public bool IsManualMode { get; set; }

    public string Recipe { get; set; }

    public IUserAttentions UserAttentions { get; } = new UserAttentions<Model>();

    public ICriticalStops CriticalStops { get; } = new CriticalStops<Model>();


    public string PlcStatusMessage { get; set; }
    public Color PlcStatusMessageBackColor { get; set; }
    [DependsOn(nameof(PlcStatusMessageBackColor))]
    public Color PlcStatusMessageForeColor => this.PlcStatusMessageBackColor.ContrastColor();

    public string MahloStatusMessage { get; set; }
    public Color MahloStatusMessageBackColor { get; set; }
    [DependsOn(nameof(MahloStatusMessageBackColor))]
    public Color MahloStatusMessageForeColor => this.MahloStatusMessageBackColor.ContrastColor();

    public string MappingStatusMessage { get; set; }
    public Color MappingStatusMessageBackColor { get; set; }
    [DependsOn(nameof(MappingStatusMessageBackColor))]
    public Color MappingStatusMessageForeColor => this.MappingStatusMessageBackColor.ContrastColor();

    [DependsOn(nameof(FeetCounterStart), nameof(FeetCounterEnd))]
    public long MeasuredLength => this.FeetCounterEnd - this.FeetCounterStart;
    public abstract long FeetCounterStart { get; set; }
    public abstract long FeetCounterEnd { get; set; }
    public abstract int Speed { get; set; }
    public abstract bool IsMapValid { get; set; }
    public double MeasuredWidth { get; set; }
    [DependsOn(nameof(MeasuredWidth))]
    public string MeasuredWidthStr => CommonMethods.InchesToStr(this.MeasuredWidth);

    public int PreviousRollLength { get; set; }
    public int RollChangesUntilCheckRequired { get; set; }
    public int StyleChangesUntilCheckRequired { get; set; }

    [DependsOn(nameof(CurrentRoll))]
    public bool CanMoveBackward => this.sewinQueue.Rolls.Contains(this.currentRoll) && this.currentRoll != this.sewinQueue.Rolls.FirstOrDefault();
    [DependsOn(nameof(CurrentRoll))]
    public bool CanMoveForward => this.sewinQueue.Rolls.Contains(this.currentRoll) && this.currentRoll != this.sewinQueue.Rolls.LastOrDefault();

    [DependsOn(nameof(UserAttentions))]
    public bool IsSeamDetectEnabled => !this.UserAttentions.IsSystemDisabled;

    public bool IsSeamDetected { get; set; }

    public bool IsDoffDetected { get; set; }

    [DependsOn(nameof(UserAttentions))]
    public bool IsSystemEnabled => !this.UserAttentions.IsSystemDisabled;

    [DependsOn(nameof(UserAttentions))]
    string IStatusBarInfo.AlertMessage =>
      this.UserAttentions.IsSystemDisabled ? "System is disabled, seams are ignored, press [Wait for Seam] to arm" :
      this.UserAttentions.IsRollTooLong ? "Current roll measured too long, verify roll sequence" :
      this.UserAttentions.IsRollTooShort ? "Previous roll measured too short, verify roll sequence" :
      this.UserAttentions.VerifyRollSequence ? "Verify roll sequence, press [Wait for Seam] to arm system" :
      "Ready for next roll seam...";

    [DependsOn(nameof(CriticalStops))]
    string IStatusBarInfo.CriticalAlarmMessage => $"{(this.CriticalStops.IsMahloCommError ? "Mahlo" : "Seam detector PLC")} is not communicating";

    [DependsOn(nameof(UserAttentions), nameof(MeasuredLength))]
    public bool IgnoringSeams => this.UserAttentions.IsSystemDisabled || this.MeasuredLength < this.serviceSettings.SeamDetectableThreshold;

    public string QueueMessage { get; set; }

    [DependsOn(nameof(IsMapValid))]
    public string LblRollMappedText => this.IsMapValid ? "Yes" : "No";
    [DependsOn(nameof(IsMapValid))]
    public Color LblRollMappedForeColor => this.IsMapValid ? Color.Green : Color.Red;

    [DependsOn(nameof(IsManualMode))]
    public string LblControllerModeText => $"Controller mode is {(this.IsManualMode ? "Manual" : "Automatic")}";
    [DependsOn(nameof(IsManualMode))]
    public Color LblControllerModeBackColor => this.IsManualMode ? Color.Yellow : Color.Green;
    [DependsOn(nameof(IsManualMode))]
    public Color LblControllerModeForeColor => this.IsManualMode ? Color.Black : Color.White;

    [DependsOn(nameof(IsMappingNow))]
    public string LblMappingStatusText => this.IsMappingNow ? "Mapping" : "Not mapping";
    [DependsOn(nameof(IsMappingNow))]
    public Color LblMappingStatusForeColor => this.IsMappingNow ? Color.Green : Color.Red;

    public void RefreshStatusDisplay()
    {
      // Mahlo status
      (Color backColor, string message) =
        this.CriticalStops.IsMahloCommError ? (Color.Red, "Mahlo is NOT communicating") :
        this.IsManualMode ? (Color.Yellow, "Mahlo is in Manual mode") :
        (Color.Green, $"Mahlo Recipe: {(string.IsNullOrWhiteSpace(this.Recipe) ? "Unknown" : this.Recipe)}");
      this.MahloStatusMessage = message;
      this.MahloStatusMessageBackColor = backColor;

      // PLC status
      (backColor, message) =
        this.CriticalStops.IsPlcCommError ? (Color.Red, "PLC is NOT Communicating") :
        (Color.Green, "PLC is Communicating");
      this.PlcStatusMessage = message;
      this.PlcStatusMessageBackColor = backColor;

      // Mapping status
      (backColor, message) =
        this.CriticalStops.Any ? (Color.Red, "Mapping is off due to one or more critical problems") :
        this.UserAttentions.IsSystemDisabled ? (Color.Yellow, "System is disabled, seams are ignored, press [F3] to arm") :
        this.UserAttentions.IsRollTooLong ? (Color.Yellow, "Measured roll length excessively long, verify roll sequence") :
        this.UserAttentions.IsRollTooShort ? (Color.Yellow, "Measured roll length excessively short, verify roll sequence") :
        this.UserAttentions.VerifyRollSequence ? (Color.Yellow, "Verify roll sequence, click \"Wait for Seam\" to arm system") :
        (Color.Green, "Roll is being mapped");
      this.MappingStatusMessage = message;
      this.MappingStatusMessageBackColor = backColor;
    }


    public Task ApplyRecipeAsync(string recipeName, bool isManualMode)
    {
      // Only used in MahloServer
      throw new NotImplementedException();
    }

    public void MoveToNextRoll(int rollLength)
    {
      this.ipcClient.CallAsync(Ipc.MahloIpcClient.MoveToNextRollCommand, this.InterfaceName, rollLength).NoWait();
    }

    public void MoveToPriorRoll()
    {
      this.ipcClient.CallAsync(Ipc.MahloIpcClient.MoveToPriorRollCommand, this.InterfaceName).NoWait();
    }

    public void Start()
    {
      throw new NotImplementedException();
    }

    public void WaitForSeam()
    {
      this.ipcClient.CallAsync(Ipc.MahloIpcClient.WaitForSeamCommand, this.InterfaceName).NoWait();
    }

    public void DisableSystem()
    {
      this.ipcClient.CallAsync(Ipc.MahloIpcClient.DisableSystemCommand, this.InterfaceName).NoWait();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.sewinQueueChangedSubscription.Dispose();
        this.updateMeterLogicSubscription.Dispose();
        this.connectionStateSubscription.Dispose();
        this.userAttentionsSubscription.Dispose();
        this.criticalStopsSubscription.Dispose();
      }
    }

    protected void OnPropertyChanged(string propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
