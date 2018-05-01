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
