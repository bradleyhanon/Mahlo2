﻿using System;
using System.ComponentModel;
using System.Linq;
using MahloService.Models;
using Newtonsoft.Json.Linq;

namespace MahloClient.Logic
{
  internal class SewinQueue : ISewinQueue
  {
    public event EventHandler Changed;

    public BindingList<GreigeRoll> Rolls { get; } = new BindingList<GreigeRoll>();

    public void UpdateSewinQueue(JArray jsonRolls)
    {
      int index = 0;
      foreach (var jsonRoll in jsonRolls)
      {
        if (index < this.Rolls.Count)
        {
          jsonRolls[index].Populate(this.Rolls[index]);
        }
        else
        {
          this.Rolls.Add(jsonRoll.ToObject<GreigeRoll>());
        }

        index++;
      }

      // Remove extra rolls
      while (this.Rolls.Count > jsonRolls.Count)
      {
        this.Rolls.RemoveAt(this.Rolls.Count - 1);
      }

      this.Changed?.Invoke(this, EventArgs.Empty);
    }

    public RollTypeEnum DetermineRollType(GreigeRoll roll)
    {
      string[] sHPBackingCodes = { "XL", "XP", "HP" };

      string sBacking1 = "", sBacking2 = "";
      double nWidth1 = 0F, nWidth2 = 0F;

      int positionInQueue = this.Rolls.IndexOf(roll);
      if (string.Equals(roll.RollNo, GreigeRoll.CheckRollId, StringComparison.OrdinalIgnoreCase))
      {
        if (positionInQueue == this.Rolls.Count - 1)
        {
          //since this is the last roll in the queue, we'll assume
          //it's the "tailout"
          return RollTypeEnum.Leader;
        }

        //search backward in queue for first non-CheckRoll
        for (int n = positionInQueue - 1; n >= 0; n--)
        {
          if (!string.Equals(this.Rolls[n].RollNo, GreigeRoll.CheckRollId, StringComparison.OrdinalIgnoreCase))
          {
            sBacking1 = this.Rolls[n].BackingCode;
            nWidth1 = this.Rolls[n].RollWidth;
            break;
          }
        }

        //search forward in queue for first non-CheckRoll
        for (int n = positionInQueue + 1; n < this.Rolls.Count; n++)
        {
          if (!string.Equals(this.Rolls[n].RollNo, GreigeRoll.CheckRollId, StringComparison.OrdinalIgnoreCase))
          {
            sBacking2 = this.Rolls[n].BackingCode;
            nWidth2 = this.Rolls[n].RollWidth;
            break;
          }
        }

        if (nWidth1 != nWidth2)
        {
          return RollTypeEnum.Leader;
        }
        else if (!sHPBackingCodes.Contains(sBacking1) && sHPBackingCodes.Contains(sBacking2))
        {
          return RollTypeEnum.Leader;
        }
        else if (!sHPBackingCodes.Contains(sBacking2) && sHPBackingCodes.Contains(sBacking1))
        {
          return RollTypeEnum.Leader;
        }
        else
        {
          return RollTypeEnum.CheckRoll;
        }
      }

      return RollTypeEnum.Greige;
    }
  }
}
