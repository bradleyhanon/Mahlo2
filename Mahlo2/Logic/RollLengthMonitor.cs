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
  class RollLengthMonitor
  {
    IAppInfoBAS appInfo;
    private bool bTurnOffStatusIndicator;
    private double nLengthWhereSeamDetected;
    private bool bMustClearUnlatchBit;
    //private double nCounterResetAtFootage;
    private bool bNotifyRollSize;
    private UserAttentionEnum nUserAttentions;
    private CriticalStopEnum nCriticalStops;

    public RollLengthMonitor(IAppInfoBAS appInfo)
    {
      this.appInfo = appInfo;
    }

    public IMeterSrc srcData { get; set; }
    public GreigeRoll CurrentGreigeRoll { get; set; }
    public MahloRoll CurrentRoll { get; set; }

    public void MetersCountChanged(double metersCount)
    {
      double feetCount = Extensions.MetersToFeet(metersCount);
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
        bRollTooLong = (this.srcData.FeetCount() > (this.CurrentGreigeRoll.RollLength * 1.1d));
      }

      if (bRollTooLong && bNotifyRollSize)
      {
        bNotifyRollSize = false;
        //modBowAndSkew.WriteToLogFile("lblMeasuredLen_Change", "Measured Len: " + rawData.MeasuredLength.ToString() + "; Roll Len: " + rawData.RollLength.ToString());
        SetUserAttention(UserAttentionEnum.attRollTooLong);
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

      if ((nUserAttentions & UserAttentionEnum.attSystemDisabled) == UserAttentionEnum.attSystemDisabled)
      {
        // Do not respond to seam detection if system is disabled
        return;
      }
      else if ((srcData.FeetCount() <= this.appInfo.SeamDetectableThreshold))
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
            bRollTooShort = (srcData.FeetCount() < (this.CurrentGreigeRoll.RollLength * 0.9d));
          }
          if ((bRollTooShort))
          {
            SetUserAttention(UserAttentionEnum.attRollTooShort);
          }
        }
      }

      if (nUserAttentions == 0 && nCriticalStops == 0)
      {
        this.BowBiasMapIsValid = true;
      }

      if (this.BowBiasMapIsValid)
      {
        this.SaveRollMap();
      }

      // Get measured footage at last seam detect
      nLengthWhereSeamDetected = srcData.FeetCount();

      // Clear the charts
      //ClearCharts();

      this.BowBiasMapIsValid = false;

      this.srcData.ResetMeterOffset();
      this.RollMap = new List<Model>();
      this.CurrentRollId++;

      this.CurrentGreigeRoll = this.sewinQueue.Rolls.FirstOrDefault(roll => roll.RollId == this.CurrentRollId);

      // Evaluate conditions that require user to re-check roll sequence
      if (!this.CurrentGreigeRoll.IsCheckRoll)
      {
        if (this.CurrentGreigeRoll.StyleCode != sPreviousStyle)
        {
          sPreviousStyle = this.CurrentGreigeRoll.StyleCode;
          nStyleCheckCount++;
        }
      }

      nRollCheckCount++;
      if (this.CurrentGreigeRoll.IsCheckRoll || (nRollCheckCount >= this.appInfo.CheckAfterHowManyRolls) || (nStyleCheckCount >= this.appInfo.CheckAfterHowManyStyles))
      {
        if ((nUserAttentions & UserAttentionEnum.attVerifyRollSequence) == UserAttentionEnum.attVerifyRollSequence)
        {
          this.BowBiasMapIsValid = false;
        }
        else
        {
          SetUserAttention(UserAttentionEnum.attVerifyRollSequence);
        }
      }

      bNotifyRollSize = true;

      ShowMappingStatus();

      if (this.CurrentGreigeRoll.IsCheckRoll)
      {
        //Automatically disable mapping if Check Roll determined to be leader
        if (this.sewinQueue.RollIsLeader(this.CurrentRollId))
        {
          DisableMapping(true);
        }
      }

      if ((nUserAttentions & UserAttentionEnum.attVerifyRollSequence) == UserAttentionEnum.attVerifyRollSequence)
      {
        dbMfg.SendEmail(this.appInfo.SendEmailAlertsTo, "Urgent: Mahlo Bow and Skew Alert!", "The operator has not responded to a system request in a timely manner." + Environment.NewLine + Environment.NewLine + "Please investigate.");
      }
    }

    private void SetUserAttention(UserAttentionEnum AttentionValue, bool Off = false, bool ShowIndicator = true)
    {
      if (Off)
      {
        if ((nUserAttentions & AttentionValue) == AttentionValue)
        {
          nUserAttentions ^= AttentionValue;
        }
      }
      else
      {
        if ((nUserAttentions & AttentionValue) != AttentionValue)
        {
          nUserAttentions |= AttentionValue;

          if (AttentionValue == UserAttentionEnum.attSystemDisabled)
          {
            //ErrorHandlingHelper.ResumeNext(
            //  // Clear certain values
            //  () => { lblMeasuredLen.Text = ""; },
            //  () => { lblLineSpeed.Text = ""; },
            //  () => { lblCarpetWidth.Text = ""; },
            //  () => { lblBowDistortionPercent.Text = ""; },
            //  () => { lblBowDistortionInches.Text = ""; },
            //  () => { lblSkewDistortionPercent.Text = ""; },
            //  () => { lblSkewDistortionInches.Text = ""; },
            //  () => { ClearCharts(); });

            SetUserAttention(UserAttentionEnum.attVerifyRollSequence, false, false);
          }
          else if (AttentionValue == UserAttentionEnum.attRollTooLong)
          {
            SetUserAttention(UserAttentionEnum.attRollTooShort, true, false);
            SetUserAttention(UserAttentionEnum.attVerifyRollSequence, false, false);
          }
          else if (AttentionValue == UserAttentionEnum.attRollTooShort)
          {
            SetUserAttention(UserAttentionEnum.attRollTooLong, true, false);
            SetUserAttention(UserAttentionEnum.attVerifyRollSequence, false, false);
          }
        }
      }

      ShowMappingStatus();
      if (ShowIndicator /*&& PLCForm != null*/)
      {
        //await PLCForm.ExecuteDDECommand(frmSLC500.DDECommandEnum.ddeSetUserAttention, (nUserAttentions > 0) ? "1" : "0");
      }
    }
  }
}
