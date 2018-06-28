using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;

namespace MahloService.Simulation
{
  class DbMfgSim : IDbMfgSim
  {
    private int nextRollNo = 1000000;
    public BindingList<GreigeRoll> SewinQueue { get; } = new BindingList<GreigeRoll>
    {
      new GreigeRoll
      {
        RollNo = GreigeRoll.CheckRollId,
        RollLength = 600,
        RollWidth = 144,
      }
    };

    public void AddRoll()
    {
      GreigeRoll roll = new GreigeRoll
      {
        RollNo = nextRollNo.ToString(),
        OrderNo = (nextRollNo + 1000000).ToString(),
        ColorCode = "001",
        ColorName = "Red",
        StyleCode = "Plaid",
        StyleName = "Checkerboard",
        BackingCode = "SA",
        DefaultRecipe = "Line Detection",
        RollWidth = 144,
        RollLength = 100,
        PatternRepeatLength = 2.35,
      };

      this.nextRollNo++;
      this.SewinQueue.Add(roll);
    }

    public Task BasUpdateDefaultRecipe(string styleCode, string rollNo, string recipeName)
    {
      if (!string.IsNullOrWhiteSpace(styleCode))
      {
        this.SewinQueue
          .Where(roll => roll.StyleCode == styleCode)
          .ForEach(roll => roll.DefaultRecipe = recipeName);
      }

      return Task.CompletedTask;
    }

    public Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence)
    {
      return Task.FromResult<IEnumerable<CoaterScheduleRoll>>(new CoaterScheduleRoll[0]);
    }

    public Task<IEnumerable<GreigeRoll>> GetCoaterSewinQueue()
    {
      return Task.FromResult<IEnumerable<GreigeRoll>>(this.SewinQueue);
    }

    public Task<AS400FinishedRoll> GetCutRollFromHost()
    {
      return Task.FromResult<AS400FinishedRoll>(new AS400FinishedRoll());
    }

    public Task<bool> GetIsSewinQueueChanged(int rowCount, string firstRollNo, string lastRollNo)
    {
      return Task.FromResult<bool>(true);
    }

    public Task<(string styleName, string colorName)> GetNamesFromLegacyCodes(string styleCode, string colorCode)
    {
      return Task.FromResult<(string, string)>((styleCode, colorCode));
    }

    public Task SendEmail(string pRecipients, string pSubject, string pBody)
    {
      return Task.CompletedTask;
    }
  }
}
