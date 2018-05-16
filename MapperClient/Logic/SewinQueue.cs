using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;

namespace MapperClient.Logic
{
  class SewinQueue : ISewinQueue
  {
    public event EventHandler Changed;

    public BindingList<CarpetRoll> Rolls { get; } = new BindingList<CarpetRoll>();

    public void UpdateSewinQueue(IEnumerable<CarpetRoll> newRolls)
    {
      var list = new List<ListItem>();

      // Update the overlapping rolls and add new rolls
      foreach (var newRoll in newRolls)
      {
        var info = this.Rolls
          .Select((oldRoll, index) => new { oldRoll, index })
          .Where(item => item.oldRoll.RollNo == newRoll.RollNo)
          .FirstOrDefault();

        if (info == null)
        {
          list.Add(new ListItem(this.Rolls.Count, newRoll));
          this.Rolls.Add(newRoll);
        }
        else
        {
          list.Add(new ListItem(info.index, info.oldRoll));
          newRoll.CopyTo(info.oldRoll);
        }
      }

      // Rearrange the rolls into the same order as the new sewin queue items
      int offset = this.Rolls.Count - list.Count;
      for (int j = list.Count - 1; j >= 0; j--)
      {
        int srcNdx = list[j].Index;
        int dstNdx = j + offset;
        if (list[j].Index != dstNdx)
        {
          // The index of the items after srcNdx (the item to be moved)
          // must be decreased by one
          for (int k = 0; k < j; k++)
          {
            if (list[k].Index > srcNdx)
            {
              list[k].Index--;
            }
          }

          // Move the roll to its new position
          var roll = this.Rolls[srcNdx];
          this.Rolls.Insert(dstNdx + 1, roll);
          this.Rolls.RemoveAt(srcNdx);
        }
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

    private class ListItem
    {
      public int Index;
      public readonly CarpetRoll Roll;

      public ListItem(int index, CarpetRoll roll)
      {
        this.Index = index;
        this.Roll = roll;
      }
    }
  }
}
