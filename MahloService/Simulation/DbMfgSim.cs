using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MahloService.Models;
using MahloService.Repository;

namespace MahloService.Simulation
{
  internal class DbMfgSim : IDbMfgSim
  {
    private int nextRollNo = 1000000;
    private int nextCutRoll = 2000000;

    public DbMfgSim(IDbLocal dbLocal)
    {
      this.SewinQueue.AddRange(dbLocal.GetRecentGreigeRolls(0));
      this.nextRollNo = dbLocal.GetNextGreigeRollId() + 1000000;
      if (this.SewinQueue.Count == 0)
      {
        this.SewinQueue.Add(new GreigeRoll
        {
          RollNo = GreigeRoll.CheckRollId,
          RollLength = 600,
          RollWidth = 144,
        });
      }
    }

    public BindingList<GreigeRoll> SewinQueue { get; } = new BindingList<GreigeRoll>();

    public void AddRoll()
    {
      GreigeRoll roll = new GreigeRoll
      {
        RollNo = this.nextRollNo.ToString(CultureInfo.InvariantCulture),
        OrderNo = (this.nextRollNo + 1000000).ToString(CultureInfo.InvariantCulture),
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

    public void RemoveRoll()
    {
      if (this.SewinQueue.Count > 0)
      {
        this.SewinQueue.RemoveAt(0);
      }
    }

    public Task BasUpdateDefaultRecipeAsync(string styleCode, string rollNo, string recipeName)
    {
      if (!string.IsNullOrWhiteSpace(styleCode))
      {
        this.SewinQueue
          .Where(roll => roll.StyleCode == styleCode)
          .ForEach(roll => roll.DefaultRecipe = recipeName);
      }

      return Task.CompletedTask;
    }

    public Task<IEnumerable<CoaterScheduleRoll>> GetCoaterScheduleAsync(int minSequence, int maxSequence)
    {
      return Task.FromResult<IEnumerable<CoaterScheduleRoll>>(Array.Empty<CoaterScheduleRoll>());
    }

    public Task<IEnumerable<GreigeRoll>> GetCoaterSewinQueueAsync()
    {
      return Task.FromResult<IEnumerable<GreigeRoll>>(this.SewinQueue);
    }

    public Task<decimal?> GetCutRollFromHostAsync()
    {
      return Task.FromResult<decimal?>(this.nextCutRoll);
    }

    public Task<bool> GetIsSewinQueueChangedAsync(int rowCount, string firstRollNo, string lastRollNo)
    {
      return Task.FromResult(
        this.SewinQueue.Count != rowCount ||
        firstRollNo != (this.SewinQueue.First()?.RollNo ?? string.Empty) ||
        lastRollNo != (this.SewinQueue.Last()?.RollNo ?? string.Empty));
    }

    public Task<(string styleName, string colorName)> GetNamesFromLegacyCodesAsync(string styleCode, string colorCode)
    {
      return Task.FromResult<(string, string)>((styleCode, colorCode));
    }

    public Task SendEmailAsync(string pRecipients, string pSubject, string pBody)
    {
      return Task.CompletedTask;
    }

    public void CutRollCompleted()
    {
      this.nextCutRoll++;
    }
  }
}
