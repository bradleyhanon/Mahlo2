using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.AppSettings;
using Mahlo.Models;
using Mahlo.Opc;

namespace Mahlo.Logic
{
  class RollLengthMonitor<Model> : IRollLengthMonitor<Model>
    where Model : MahloRoll
  {
    private IUserAttentions<Model> userAttentions;
    IAppInfoBAS appInfo;
    private bool bTurnOffStatusIndicator;
    private double nLengthWhereSeamDetected;
    private bool bMustClearUnlatchBit;
    //private double nCounterResetAtFootage;
    private bool bNotifyRollSize;
   

    public RollLengthMonitor(IUserAttentions<Model> userAttentions, IAppInfoBAS appInfo)
    {
      this.userAttentions = userAttentions;
      this.appInfo = appInfo;
    }

    public IMeterSrc<Model> srcData { get; set; }
    public GreigeRoll CurrentGreigeRoll { get; set; }
    public MahloRoll CurrentRoll { get; set; }

    public void MetersCountChanged(int feetCount)
    {
      if (bTurnOffStatusIndicator && feetCount != nLengthWhereSeamDetected && feetCount >= this.appInfo.SeamDetectableThreshold)
      {
        srcData.SetStatusIndicator(false);
        bTurnOffStatusIndicator = false;
      }

      bool bRollTooLong = false;
      //if (bMustClearUnlatchBit && feetCount >= appInfo.SeamDetectableThreshold && feetCount < nCounterResetAtFootage)
      if (bMustClearUnlatchBit && feetCount >= appInfo.SeamDetectableThreshold)
      {
        this.srcData.ResetSeamDetector();
        bMustClearUnlatchBit = false;
      }

      if (CurrentGreigeRoll.RollLength >= 100)
      {
        bRollTooLong = (feetCount > (this.CurrentGreigeRoll.RollLength * 1.1d));
      }

      if (bRollTooLong && bNotifyRollSize)
      {
        bNotifyRollSize = false;
        //modBowAndSkew.WriteToLogFile("lblMeasuredLen_Change", "Measured Len: " + rawData.MeasuredLength.ToString() + "; Roll Len: " + rawData.RollLength.ToString());
        this.userAttentions.IsRollTooLong = true;
      }
    }

    public void SeamDetected()
    {
      //CarpetRoll oGreigeRoll = new CarpetRoll();
      //ADORecordSetHelper rsRollMap = null;
      bool bRollTooShort = false;

      //nCounterResetAtFootage = this.CurrentRoll.Feet;
      //if (nCounterResetAtFootage <= this.appInfo.SeamDetectableThreshold || Math.Abs(nCounterResetAtFootage - this.appInfo.SeamDetectableThreshold) < 20)
      //{
      //  nCounterResetAtFootage = 100;
      //}

      bMustClearUnlatchBit = true;

      srcData.SetStatusIndicator(true);
      bTurnOffStatusIndicator = true;

      if (this.userAttentions.IsSystemDisabled)
      {
        // Do not respond to seam detection if system is disabled
        return;
      }
      else if ((this.CurrentRoll.Feet <= this.appInfo.SeamDetectableThreshold))
      {
        // Do not respond to seam if footage is below threshold, could be detecting same seam
        return;
      }
      else if (this.CurrentGreigeRoll != null)
      {
        //if (rawData.BowBiasMapIsValid)
        {
          if (this.CurrentGreigeRoll.RollLength >= 100)
          {
            bRollTooShort = (this.CurrentRoll.Feet < (this.CurrentGreigeRoll.RollLength * 0.9d));
          }
          if (bRollTooShort)
          {
            this.userAttentions.IsRollTooShort = true;
          }
        }
      }

      // Get measured footage at last seam detect
      nLengthWhereSeamDetected = this.CurrentRoll.Feet;

      //if (nUserAttentions == 0 && nCriticalStops == 0)
      //{
      //  this.BowBiasMapIsValid = true;
      //}

      //if (this.BowBiasMapIsValid)
      //{
      //  this.SaveRollMap();
      //}

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
  }
}
