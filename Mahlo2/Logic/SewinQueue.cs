using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mahlo.Models;
using Mahlo.Repository;

namespace Mahlo.Logic
{
  class SewinQueue : ISewinQueue
  {
    private TimeSpan refreshInterval = TimeSpan.FromSeconds(10);
    private int firstRollId;
    private int nextRollId = 1;

    private IDbLocal dbLocal;
    private IDbMfg dbMfg;

    private bool isRefreshBusy;

    private string priorFirstRoll = string.Empty;
    private string priorLastRoll = string.Empty;
    private int priorQueueSize;

    public SewinQueue(IDbLocal dbLocal, IDbMfg dbMfg)
    {
      this.dbLocal = dbLocal;
      this.dbMfg = dbMfg;

      this.Rolls = new ObservableCollection<GreigeRoll>(this.dbLocal.GetGreigeRolls());
      //this.RunAutoRefresh();
    }

    public ObservableCollection<GreigeRoll> Rolls { get; private set; } = new ObservableCollection<GreigeRoll>();

    public bool RollIsLeader(int rollId)
    {
      var TheRoll = this.Rolls.Single(item => item.RollId == rollId);

      bool result = false;
      try
      {

        //RollQueue theQueue = new RollQueue();
        //int bk = -1;
        string sBk1 = "", sBk2 = "";
        double nWidth1 = 0, nWidth2 = 0;

        //bk = TheRoll.QueueBookmark;
        // TODO: Something seems wrong: It seems SewinQueue should be cloned instead of SewinQueue.QueueRS
        //theQueue = (RollQueue)SewinQueue.QueueRS.Clone();
        //theQueue.QueueRS = SewinQueue.QueueRS.Clone();

        //theQueue.QueueRS.setBookmark(bk);
        //theQueue.QueueRS.MovePrevious();

        int index = rollId;
        //while (!theQueue.QueueRS.BOF)
        while (--index >= this.firstRollId)
        {
          var roll = this.Rolls[index - this.firstRollId];
          if (roll.G2ROLL != GreigeRoll.CheckRollId)
          {
            sBk1 = roll.G2SBK;
            nWidth1 =  Convert.ToDouble(roll.G2WTF) * 12 + roll.G2WTI; 
            break;
          }
        };

        //theQueue.QueueRS.Bookmark = ReflectionHelper.GetPrimitiveValue<DataRow>(bk);
        //theQueue.QueueRS.MoveNext();

        index = rollId;
        //while (!theQueue.QueueRS.EOF)
        while (++index < nextRollId)
        {
          var roll = this.Rolls[index - this.firstRollId];
          if (roll.G2ROLL != GreigeRoll.CheckRollId)
          {
            sBk2 = roll.G2SBK;
            nWidth2 = Convert.ToDouble(roll.G2WTF) * 12 + roll.G2WTI;
            break;
          }
        };

        result = (sBk1 == "XL" && sBk2 != "XL") || nWidth1 != nWidth2;
      }
      catch
      {
      }

      return result;
    }



    private async void RunAutoRefresh()
    {
      while (true)
      {
        await this.Refresh();
        await Task.Delay(this.refreshInterval);
      }
    }

    private async Task Refresh()
    {
      if (this.isRefreshBusy)
      {
        return;
      }

      this.isRefreshBusy = true;
      try
      {
        if (await this.dbMfg.GetIsSewinQueueChanged(this.priorQueueSize, this.priorFirstRoll, this.priorLastRoll))
        {
          var newRolls = (await this.dbMfg.GetCoaterSewinQueue()).ToArray();
          this.priorFirstRoll = newRolls.FirstOrDefault()?.G2ROLL ?? string.Empty;
          this.priorLastRoll = newRolls.LastOrDefault()?.G2ROLL ?? string.Empty;
          this.priorQueueSize = newRolls.Count();

          foreach (var newRoll in newRolls)
          {
            var oldRoll = this.Rolls.FirstOrDefault(item => item.G2ROLL == newRoll.G2ROLL);
            if (oldRoll != null)
            {
              newRoll.CopyTo(oldRoll);
              dbLocal.UpdateGreigeRoll(oldRoll);
            }
            else
            {
              newRoll.RollId = this.nextRollId++;
              this.Rolls.Add(newRoll);
              dbLocal.AddGreigeRoll(newRoll);
            }
          }

          this.firstRollId = this.Rolls.Min(item => item.RollId);
        }
      }
      catch (Exception ex)
      {

      }

      this.isRefreshBusy = false;
    }
  }
}