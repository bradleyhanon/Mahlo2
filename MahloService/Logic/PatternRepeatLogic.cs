using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Settings;
using MahloService.Models;
using MahloService.Opc;
using MahloService.Repository;
using MahloService.Utilities;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Collections.Specialized;

namespace MahloService.Logic
{
  class PatternRepeatLogic : MeterLogic<PatternRepeatRoll>, IPatternRepeatLogic
  {
    IServiceSettings appInfo;
    private IPatternRepeatSrc srcData;
    private ICutRollList cutRolls;

    private double maxElongation;
    private bool isDoffAckNeeded;
    private int feetCounterAtCutRollStart;

    public PatternRepeatLogic(
      IPatternRepeatSrc srcData,
      ISewinQueue sewinQueue,
      ICutRollList cutRolls,
      IServiceSettings appInfo,
      IUserAttentions<PatternRepeatRoll> userAttentions,
      ICriticalStops<PatternRepeatRoll> criticalStops,
      IProgramState programState,
      ISchedulerProvider schedulerProvider)
      : base(srcData, sewinQueue, appInfo, userAttentions, criticalStops, programState, schedulerProvider)
    {
      this.appInfo = appInfo;
      this.srcData = srcData;
      this.cutRolls = cutRolls;

      this.Disposables.Add(
        Observable
        .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
          h => this.cutRolls.CollectionChanged += h,
          h => this.cutRolls.CollectionChanged -= h)
        .Subscribe(_ =>
        {
          this.CurrentCutRoll = this.cutRolls.Last();
        }));
    }

    [JsonIgnore]
    public CutRoll CurrentCutRoll { get; set; }

    public bool IsDoffDetected { get; set; }

    public override int FeetCounterStart
    {
      get => this.CurrentRoll.PrsFeetCounterStart;
      set => this.CurrentRoll.PrsFeetCounterStart = value;
    }

    public override int FeetCounterEnd
    {
      get => this.CurrentRoll.PrsFeetCounterEnd;
      set => this.CurrentRoll.PrsFeetCounterEnd = value;
    }

    public override int Speed
    {
      get => this.CurrentRoll.PrsSpeed;
      set => this.CurrentRoll.PrsSpeed = value;
    }

    public override bool IsMapValid
    {
      get => this.CurrentRoll.PrsMapValid;
      set => this.CurrentRoll.PrsMapValid = value;
    }

    protected override void OnRollFinished(GreigeRoll greigeRoll)
    {
      base.OnRollFinished(greigeRoll);
      greigeRoll.Elongation = this.maxElongation;
    }

    protected override void OnRollStarted(GreigeRoll greigeRoll)
    {
      base.OnRollStarted(greigeRoll);
      this.maxElongation = 0;

      this.feetCounterAtCutRollStart = (int)this.srcData.FeetCounter;
    }

    protected override void OpcValueChanged(string propertyName)
    {
      switch (propertyName)
      {
        case nameof(this.srcData.PatternRepeatLength):
          this.CurrentRoll.PatternRepeatLength = this.srcData.PatternRepeatLength;
          double elongation = srcData.PatternRepeatLength - this.CurrentRoll.PatternRepeatLength;
          if (Math.Abs(elongation) > Math.Abs(this.maxElongation))
          {
            this.maxElongation = elongation;
          }

          break;

        case nameof(this.srcData.IsDoffDetected):
          this.DoffDetected();
          break;

        default:
          base.OpcValueChanged(propertyName);
          break;
      }
    }

    protected override void FeetCounterChanged(int feetCounter)
    {
      base.FeetCounterChanged(feetCounter);

      this.CurrentCutRoll.FeetCounterEnd = feetCounter;
      if (this.CurrentCutRoll.Length < this.appInfo.MinSeamSpacing)
      {
        return;
      }

      if (this.isDoffAckNeeded)
      {
        this.isDoffAckNeeded = false;
        this.srcData.AcknowledgeDoffDetect();
        this.IsDoffDetected = false;
      }

      if (this.CurrentCutRoll == null)
      {
        this.CurrentCutRoll = new CutRoll();
        this.cutRolls.Add(this.CurrentCutRoll);
      }
    }

    private void DoffDetected()
    {
      if (!this.srcData.IsDoffDetected)
      {
        return;
      }

      this.isDoffAckNeeded = true;

      // Ignore doff if system is disabled
      if (this.UserAttentions.IsSystemDisabled)
      {
        return;
      }

      if (this.CurrentCutRoll == null)
      {
        return;
      }

      if (this.IsMapValid)
      {
        this.SaveCutRollMap();
      }

      // CutRoll is finished
      this.CurrentCutRoll = new CutRoll
      {
        FeetCounterStart = (int)this.srcData.FeetCounter,
        FeetCounterEnd = (int)this.srcData.FeetCounter,
      };

      this.cutRolls.Add(this.CurrentCutRoll);

      this.IsDoffDetected = true;
    }

    private void SaveCutRollMap()
    {

    }
  }
}
