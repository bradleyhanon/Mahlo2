using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Logic;
using Mahlo.Models;
using PropertyChanged;

namespace MapperClient.Logic
{
  [AddINotifyPropertyChangedInterface]
  abstract class ModelLogic : IModelLogic
  {
    private ISewinQueue sewinQueue;
    private string currentRollNo = string.Empty;
    private IDisposable SewinQueueChangedSubscription;

    public ModelLogic(ISewinQueue sewinQueue)
    {
      this.sewinQueue = sewinQueue;
      this.SewinQueueChangedSubscription =
        Observable.FromEventPattern<EventHandler, EventArgs>(
          h => this.sewinQueue.Changed += h,
          h => this.sewinQueue.Changed -= h)
          .Subscribe(args =>
          {
            this.CurrentRoll = this.sewinQueue.Rolls.FirstOrDefault(roll => roll.RollNo == this.CurrentRollNo) ?? new CarpetRoll();
          });
    }

    public bool IsChanged { get; set; }
    public CarpetRoll CurrentRoll { get; set; } = new CarpetRoll();
    public string CurrentRollNo
    {
      get => this.currentRollNo;
      set
      {
        this.currentRollNo = value;
        this.CurrentRoll = sewinQueue.Rolls.FirstOrDefault(roll => roll.RollNo == value) ?? new CarpetRoll();
      }
    }

    public bool IsManualMode { get; set; }

    public string Recipe { get; set; }

    public IUserAttentions UserAttentions { get; set; } = new UserAttentions();

    public ICriticalStops CriticalStops { get; set; } = new CriticalStops();


    public IObservable<CarpetRoll> RollStarted => throw new NotImplementedException();

    public IObservable<CarpetRoll> RollFinished => throw new NotImplementedException();

    public string PlcStatusMessage { get; set; }
    public Color PlcStatusMessageBackColor { get; set; }
    [DependsOn(nameof(PlcStatusMessageBackColor))]
    public Color PlcStatusMessageForeColor => PlcStatusMessageBackColor.ContrastColor();

    public string MahloStatusMessage { get; set; }
    public Color MahloStatusMessageBackColor { get; set; }
    [DependsOn(nameof(MahloStatusMessageBackColor))]
    public Color MahloStatusMessageForeColor => MahloStatusMessageBackColor.ContrastColor();

    public string MappingStatusMessage { get; set; }
    public Color MappingStatusMessageBackColor { get; set; }
    [DependsOn(nameof(MappingStatusMessageBackColor))]
    public Color MappingStatusMessageForeColor => MappingStatusMessageBackColor.ContrastColor();

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

    public void MoveToNextRoll()
    {
      throw new NotImplementedException();
    }

    public void MoveToPriorRoll()
    {
      throw new NotImplementedException();
    }

    public void Start()
    {
      throw new NotImplementedException();
    }

    public void WaitForSeam()
    {
      throw new NotImplementedException();
    }
  }
}
