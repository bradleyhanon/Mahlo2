using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;

namespace MahloService.Simulation
{
  class DbMfgSim : IDbMfg
  {
    List<CarpetRoll> SewinQueue { get; } = new List<CarpetRoll>
    {
      new CarpetRoll
      {
        RollNo = CarpetRoll.CheckRollId,
        RollLength = 1000,
        RollWidth = 144,
      }
    };

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

    public Task<IEnumerable<CarpetRoll>> GetCoaterSewinQueue()
    {
      return Task.FromResult<IEnumerable<CarpetRoll>>(this.SewinQueue);
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
