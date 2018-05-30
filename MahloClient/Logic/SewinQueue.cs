using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloClient.Logic
{
  class SewinQueue : ISewinQueue
  {
    public event EventHandler Changed;

    public BindingList<CarpetRoll> Rolls { get; } = new BindingList<CarpetRoll>();

    public void UpdateSewinQueue(IEnumerable<CarpetRoll> newRolls)
    {
      int index = 0;
      foreach (var roll in newRolls)
      {
        if (index < this.Rolls.Count)
        {
          this.Rolls[index] = roll;
        }
        else
        {
          this.Rolls.Add(roll);
        }

        index++;
      }

      this.Changed?.Invoke(this, EventArgs.Empty);
    }

    public CarpetRollTypeEnum DetermineRollType(CarpetRoll roll)
    {
      string[] sHPBackingCodes = { "XL", "XP", "HP" };

      string sBacking1 = "", sBacking2 = "";
      double nWidth1 = 0F, nWidth2 = 0F;

      int positionInQueue = this.Rolls.IndexOf(roll);
      if (roll.RollNo.ToUpper() == CarpetRoll.CheckRollId)
      {
        if (positionInQueue == this.Rolls.Count - 1)
        {
          //since this is the last roll in the queue, we'll assume
          //it's the "tailout"
          return CarpetRollTypeEnum.Leader;
        }

        //search backward in queue for first non-CheckRoll
        for (int n = positionInQueue - 1; n >= 0; n--)
        {
          if (this.Rolls[n].RollNo.ToUpper() != CarpetRoll.CheckRollId)
          {
            sBacking1 = this.Rolls[n].BackingCode;
            nWidth1 = this.Rolls[n].RollWidth;
            break;
          }
        }

        //search forward in queue for first non-CheckRoll
        for (int n = positionInQueue + 1; n < this.Rolls.Count; n++)
        {
          if (this.Rolls[n].RollNo.ToUpper() != CarpetRoll.CheckRollId)
          {
            sBacking2 = this.Rolls[n].BackingCode;
            nWidth2 = this.Rolls[n].RollWidth;
            break;
          }
        }

        if (nWidth1 != nWidth2)
          return CarpetRollTypeEnum.Leader;
        else if (!sHPBackingCodes.Contains(sBacking1) && sHPBackingCodes.Contains(sBacking2))
          return CarpetRollTypeEnum.Leader;
        else if (!sHPBackingCodes.Contains(sBacking2) && sHPBackingCodes.Contains(sBacking1))
          return CarpetRollTypeEnum.Leader;
        else
          return CarpetRollTypeEnum.CheckRoll;
      }

      return CarpetRollTypeEnum.Greige;
    }
  }
}
