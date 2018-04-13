using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;
using Mahlo.Repository;

namespace Mahlo.Logic
{
  class MeterLogic<Model> : IMeterLogic<Model>
    where Model : MahloRoll, new()
  {
    private IDbMfg dbMfg;
    private IDbLocal dbLocal;
    private ISewinQueue sewinQueue;
    private IMeterSrc<Model> srcData;
    private IAppInfoBAS appInfo;

    IDisposable meterCountSubscription;
    IDisposable seamDetectedSubscription;

    private IRollLengthMonitor<Model> rollLengthMonitor;

    private bool bMustClearUnlatchBit;
    private bool bNotifyRollSize;
    private double nLengthWhereSeamDetected;
    private double nCounterResetAtFootage;
    private bool bTurnOffStatusIndicator = false;
    private bool BowBiasMapIsValid;
    private int nRollCheckCount;
    private string sPreviousStyle = string.Empty;
    private int nStyleCheckCount;

    public MeterLogic(IMeterSrc<Model> srcData, ISewinQueue sewinQueue, IDbMfg dbMfg, IDbLocal dbLocal, IAppInfoBAS appInfo,
      IRollLengthMonitor<Model> rollLengthMonitor,
      IUserAttentions<Model> userAttentions, ICriticalStops<Model> criticalStops)
    {
      this.dbMfg = dbMfg;
      this.dbLocal = dbLocal;
      this.sewinQueue = sewinQueue;
      this.srcData = srcData;
      this.appInfo = appInfo;
      this.UserAttentsions = userAttentions;
      this.CriticalStops = criticalStops;

      userAttentions.MeterSrc = 
        criticalStops.MeterSrc = srcData;
    }

    /// <summary>
    /// Gets the current greige roll.
    /// </summary>
    public GreigeRoll CurrentGreigeRoll { get; private set; }

    /// <summary>
    /// Get the roll that is currently being processed
    /// </summary>
    public Model CurrentRoll { get; private set; } = new Model();

    /// <summary>
    /// Gets the list of rolls that have been processed and includes the current roll at the end
    /// </summary>
    public List<Model> Rolls { get; private set; } = new List<Model>();

    /// <summary>
    /// Gets the roll map for the current roll
    /// </summary>
    public List<Model> RollMap { get; private set; } = new List<Model>();

    public int CurrentRollId { get; private set; }

    public IUserAttentions<Model> UserAttentsions { get; }

    public ICriticalStops<Model> CriticalStops { get; }

    /// <summary>
    /// Called to start data collection
    /// </summary>
    public void Start()
    {
      //this.srcData.PropertyChanged -= SrcData_PropertyChanged;
      //this.srcData.PropertyChanged += SrcData_PropertyChanged;

      this.meterCountSubscription = this.srcData.MeterCountObservable
        .Select(meters => (int)Extensions.MetersToFeet(meters))
        .DistinctUntilChanged()
        //.Do(feet => this.rollL)
        .Subscribe(feet => this.MetersCountChanged(feet));

      this.seamDetectedSubscription = this.srcData.SeamDetectedObservable
        .Where(value => value == true)
        .Subscribe(value => this.SeamDetected());
        

      this.LoadRolls();
      if (this.Rolls.Any())
      {
        this.CurrentRoll = this.Rolls.Last();
      }
      else
      {
        this.Rolls.Add(this.CurrentRoll);
      }
    }

    //private void SrcData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //  switch (e.PropertyName)
    //  {
    //    case nameof(srcData.MetersCount):
    //      this.MetersCountChanged(srcData.MetersCount);
    //      break;

    //    case nameof(srcData.SeamDetected):
    //      this.SeamDetected();
    //      break;
    //  }
    //}

    private void MetersCountChanged(int feet)
    {
      //if (bTurnOffStatusIndicator && this.srcData.FeetCount() != nLengthWhereSeamDetected && this.srcData.FeetCount() >= this.appInfo.SeamDetectableThreshold)
      //{
      //  //await PLCForm.ExecuteDDECommand(frmSLC500.DDECommandEnum.ddeSetStatusIndicator, "0");
      //  bTurnOffStatusIndicator = false;
      //}

      this.CurrentRoll.Feet = feet;

      //bool bRollTooLong = false;
      //if (bMustClearUnlatchBit && srcData.FeetCount() >= appInfo.SeamDetectableThreshold && srcData.FeetCount() < nCounterResetAtFootage)
      //{
      //  this.srcData.ResetSeamDetector();
      //  bMustClearUnlatchBit = false;
      //}
      ////if (rawData.Feet == 0)
      ////{
      ////  ClearCharts();
      ////}
      ////else
      ////{
      ////  CheckChartScale(BowChart, rawData.MeasuredLength);
      ////  CheckChartScale(SkewChart, rawData.MeasuredLength);
      ////  if ((lblBowDistortionPercent.Text != ""))
      ////  {
      ////    UpdateChart(BowChart, rawData.MeasuredLength, Double.Parse(lblBowDistortionPercent.Text));
      ////  }
      ////  if ((lblSkewDistortionPercent.Text != ""))
      ////  {
      ////    UpdateChart(SkewChart, rawData.MeasuredLength, Double.Parse(lblSkewDistortionPercent.Text));
      ////  }
      ////}

      ////if (rawData.BowBiasMapIsValid)
      //{
      //  if (CurrentGreigeRoll.RollLength >= 100)
      //  {
      //    bRollTooLong = (this.srcData.FeetCount() > (this.CurrentGreigeRoll.RollLength * 1.1d));
      //  }
      //  if (bRollTooLong && bNotifyRollSize)
      //  {
      //    bNotifyRollSize = false;
      //    //modBowAndSkew.WriteToLogFile("lblMeasuredLen_Change", "Measured Len: " + rawData.MeasuredLength.ToString() + "; Roll Len: " + rawData.RollLength.ToString());
      //    SetUserAttention(UserAttentionEnum.attRollTooLong);
      //  }
      //}
    }

    protected virtual void SeamDetected()
    {
      ////CarpetRoll oGreigeRoll = new CarpetRoll();
      ////ADORecordSetHelper rsRollMap = null;
      //bool bRollTooShort = false;

      //nCounterResetAtFootage = srcData.FeetCount();
      //if (nCounterResetAtFootage <= this.appInfo.SeamDetectableThreshold || Math.Abs(nCounterResetAtFootage - this.appInfo.SeamDetectableThreshold) < 20)
      //{
      //  nCounterResetAtFootage = 100;
      //}

      //bMustClearUnlatchBit = true;

      ////await PLCForm.ExecuteDDECommand(frmSLC500.DDECommandEnum.ddeSetStatusIndicator, "1");
      //bTurnOffStatusIndicator = true;

      //if ((nUserAttentions & UserAttentionEnum.attSystemDisabled) == UserAttentionEnum.attSystemDisabled)
      //{
      //  // Do not respond to seam detection if system is disabled
      //  return;
      //}
      //else if ((srcData.FeetCount() <= this.appInfo.SeamDetectableThreshold))
      //{
      //  // Do not respond to seam if footage is below threshold, could be detecting same seam
      //  return;
      //}
      //else if (this.CurrentGreigeRoll != null)
      //{
      //  //if (rawData.BowBiasMapIsValid)
      //  {
      //    if (this.CurrentGreigeRoll.RollLength >= 100)
      //    {
      //      bRollTooShort = (srcData.FeetCount() < (this.CurrentGreigeRoll.RollLength * 0.9d));
      //    }
      //    if ((bRollTooShort))
      //    {
      //      SetUserAttention(UserAttentionEnum.attRollTooShort);
      //    }
      //  }
      //}

      //if (nUserAttentions == 0 && nCriticalStops == 0)
      //{
      //  this.BowBiasMapIsValid = true;
      //}

      //if (this.BowBiasMapIsValid)
      //{
      //  this.SaveRollMap();
      //}

      //// Get measured footage at last seam detect
      //nLengthWhereSeamDetected = srcData.FeetCount();

      //// Clear the charts
      ////ClearCharts();

      //this.BowBiasMapIsValid = false;

      //this.srcData.ResetMeterOffset();
      //this.RollMap = new List<Model>();
      //this.CurrentRollId++;

      //this.CurrentGreigeRoll = this.sewinQueue.Rolls.FirstOrDefault(roll => roll.RollId == this.CurrentRollId);

      //// Evaluate conditions that require user to re-check roll sequence
      //if (!this.CurrentGreigeRoll.IsCheckRoll)
      //{
      //  if (this.CurrentGreigeRoll.StyleCode != sPreviousStyle)
      //  {
      //    sPreviousStyle = this.CurrentGreigeRoll.StyleCode;
      //    nStyleCheckCount++;
      //  }
      //}

      //nRollCheckCount++;
      //if (this.CurrentGreigeRoll.IsCheckRoll || (nRollCheckCount >= this.appInfo.CheckAfterHowManyRolls) || (nStyleCheckCount >= this.appInfo.CheckAfterHowManyStyles))
      //{
      //  if ((nUserAttentions & UserAttentionEnum.attVerifyRollSequence) == UserAttentionEnum.attVerifyRollSequence)
      //  {
      //    this.BowBiasMapIsValid = false;
      //  }
      //  else
      //  {
      //    SetUserAttention(UserAttentionEnum.attVerifyRollSequence);
      //  }
      //}

      //bNotifyRollSize = true;

      //ShowMappingStatus();

      //if (this.CurrentGreigeRoll.IsCheckRoll)
      //{
      //  //Automatically disable mapping if Check Roll determined to be leader
      //  if (this.sewinQueue.RollIsLeader(this.CurrentRollId))
      //  {
      //    DisableMapping(true);
      //  }
      //}

      //if ((nUserAttentions & UserAttentionEnum.attVerifyRollSequence) == UserAttentionEnum.attVerifyRollSequence)
      //{
      //  dbMfg.SendEmail(this.appInfo.SendEmailAlertsTo, "Urgent: Mahlo Bow and Skew Alert!", "The operator has not responded to a system request in a timely manner." + Environment.NewLine + Environment.NewLine + "Please investigate.");
      //}
    }

    private void LoadRolls()
    {
      this.Rolls.AddRange(this.dbLocal.GetRolls<Model>());
    }

    private void DisableMapping(bool ShowIndicator = true)
    {
      //Invalidate the map for the current roll
      this.BowBiasMapIsValid = false;

      this.UserAttentsions.IsSystemDisabled = true;
    }


    private void ShowMappingStatus()
    {
      throw new NotImplementedException();
    }

    private bool GetCurrentRollMapStatus()
    {
      return this.BowBiasMapIsValid;
    }

    public void ShowMahloStatus()
    {
      ////if (this.InvokeRequired)
      ////{
      ////  this.Invoke(new Action(ShowMahloStatus));
      ////  return;
      ////}

      //Color bColor = Color.Black;
      //Color fColor = Color.Black;
      //if ((nCriticalStops & CriticalStopEnum.stpMahloCommError) == CriticalStopEnum.stpMahloCommError)
      //{
      //  bColor = Color.Red;
      //  fColor = Color.White;
      //  //lblMahloStatus.Text = "Mahlo is NOT Communicating";
      //}
      //else
      //{
      //  if (this.rawData.OnOff)
      //  {
      //    bColor = Color.Yellow;
      //    //lblMahloStatus.Text = "Mahlo is in Manual mode";
      //  }
      //  else
      //  {
      //    bColor = Color.Green;
      //    //if (MahloForm.CurrentRecipe == "")
      //    //{
      //    //  lblMahloStatus.Text = "Mahlo Recipe: Unknown";
      //    //}
      //    //else
      //    //{
      //    //  lblMahloStatus.Text = "Mahlo Recipe: " + MahloForm.CurrentRecipe;
      //    //}
      //  }
      //}

      ////lblMahloStatus.ForeColor = fColor;
      ////lblMahloStatus.BackColor = bColor;
    }

    public void ShowPLCStatus()
    {
      //Color bColor = Color.Black;
      //Color fColor = Color.Black;
      //if ((nCriticalStops & CriticalStopEnum.stpPLCCommError) == CriticalStopEnum.stpPLCCommError)
      //{
      //  bColor = Color.Red;
      //  fColor = Color.White;
      //  lblPLCStatus.Text = "PLC is NOT Communicating";
      //}
      //else
      //{
      //  bColor = Color.Green;
      //  lblPLCStatus.Text = "PLC is Communicating";
      //}

      //lblPLCStatus.ForeColor = fColor;
      //lblPLCStatus.BackColor = bColor;
    }
  }
}
