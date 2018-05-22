using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;
using PropertyChanged;

namespace MahloClient.Logic
{
  [AddINotifyPropertyChangedInterface]
  abstract class MeterLogic<Model> : IMeterLogic
  {
    private ISewinQueue sewinQueue;
    private string currentRollNo = string.Empty;
    private IDisposable SewinQueueChangedSubscription;
    private CarpetRoll currentRoll = new CarpetRoll();

    public MeterLogic(ISewinQueue sewinQueue)
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
    public CarpetRoll CurrentRoll
    {
      get => currentRoll;
      set
      {
        this.currentRoll = value;
        this.CurrentRollType = this.sewinQueue.DetermineRollType(this.currentRoll);
        this.NextRoll = this.sewinQueue.Rolls.SkipWhile(roll => roll != this.CurrentRoll).Skip(1).FirstOrDefault();
        this.NextRollType = this.NextRoll == null ? (CarpetRollTypeEnum?)null : this.sewinQueue.DetermineRollType(this.NextRoll);
      }
    }

    public CarpetRoll NextRoll { get; set; } = new CarpetRoll();

    public string CurrentRollNo
    {
      get => this.currentRollNo;
      set
      {
        this.currentRollNo = value;
        this.CurrentRoll = this.sewinQueue.Rolls.FirstOrDefault(roll => roll.RollNo == value) ?? new CarpetRoll();
      }
    }

    public CarpetRollTypeEnum CurrentRollType { get; set; }
    public CarpetRollTypeEnum? NextRollType { get; set; }

    public bool IsManualMode { get; set; }

    public string Recipe { get; set; }

    public IUserAttentions UserAttentions { get; } = new UserAttentions<Model>();

    public ICriticalStops CriticalStops { get; } = new CriticalStops<Model>();


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

    public string RollMappedText => this.IsMapValid ? "Yes" : "No";
    public Color RollMappedBackColor => this.IsMapValid ? Color.Green : Color.Red;
    public Color RollMappedForeColor => this.RollMappedBackColor.ContrastColor();

    public abstract int Feet { get; set; }
    public abstract int Speed { get; set; }
    public abstract bool IsMapValid { get; set; }

    public int PreviousRollLength { get; set; }
    public int RollChangesUntilCheckRequired { get; set; }
    public int StyleChangesUntilCheckRequired { get; set; }

    [DependsOn(nameof(CurrentRoll))]
    public bool CanMoveBackward => this.sewinQueue.Rolls.Contains(this.currentRoll) && this.currentRoll != this.sewinQueue.Rolls.FirstOrDefault();
    [DependsOn(nameof(CurrentRoll))]
    public bool CanMoveForward => this.sewinQueue.Rolls.Contains(this.currentRoll) && this.currentRoll != this.sewinQueue.Rolls.LastOrDefault();

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
