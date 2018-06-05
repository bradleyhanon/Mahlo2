using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Logic;
using MahloService.Models;

namespace MahloService
{
  static class CommonMethods
  {
    public static string InchesToStr(double totalInches)
    {
      int feet = (int)totalInches / 12;
      int inches = (int)totalInches % 12;
      return $"{feet}' {inches}\"";
    }

    public static CarpetRollTypeEnum DetermineRollType(IList<CarpetRoll> rolls, CarpetRoll roll)
    {
      string[] sHPBackingCodes = { "XL", "XP", "HP" };

      string sBacking1 = "", sBacking2 = "";
      double nWidth1 = 0F, nWidth2 = 0F;

      int positionInQueue = rolls.IndexOf(roll);
      if (roll.RollNo.ToUpper() == CarpetRoll.CheckRollId)
      {
        if (positionInQueue == rolls.Count - 1)
        {
          //since this is the last roll in the queue, we'll assume
          //it's the "tailout"
          return CarpetRollTypeEnum.Leader;
        }

        //search backward in queue for first non-CheckRoll
        for (int n = positionInQueue - 1; n >= 0; n--)
        {
          if (rolls[n].RollNo.ToUpper() != CarpetRoll.CheckRollId)
          {
            sBacking1 = rolls[n].BackingCode;
            nWidth1 = rolls[n].RollWidth;
            break;
          }
        }

        //search forward in queue for first non-CheckRoll
        for (int n = positionInQueue + 1; n < rolls.Count; n++)
        {
          if (rolls[n].RollNo.ToUpper() != CarpetRoll.CheckRollId)
          {
            sBacking2 = rolls[n].BackingCode;
            nWidth2 = rolls[n].RollWidth;
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
