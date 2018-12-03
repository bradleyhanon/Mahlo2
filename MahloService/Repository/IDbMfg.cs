using System.Collections.Generic;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Repository
{
  internal interface IDbMfg
  {
    Task<IEnumerable<GreigeRoll>> GetCoaterSewinQueueAsync();
    Task<bool> GetIsSewinQueueChangedAsync(int rowCount, string firstRollNo, string lastRollNo);
    Task<(string styleName, string colorName)> GetNamesFromLegacyCodesAsync(string styleCode, string colorCode);
    Task BasUpdateDefaultRecipeAsync(string styleCode, string rollNo, string recipeName);
    Task<IEnumerable<CoaterScheduleRoll>> GetCoaterScheduleAsync(int minSequence, int maxSequence);
    Task SendEmailAsync(string pRecipients, string pSubject, string pBody);
    Task<decimal?> GetCutRollFromHostAsync();
  }
}