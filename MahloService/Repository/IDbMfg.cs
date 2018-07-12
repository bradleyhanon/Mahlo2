using System.Collections.Generic;
using System.Threading.Tasks;
using MahloService.Models;

namespace MahloService.Repository
{
  interface IDbMfg
  {
    Task<IEnumerable<GreigeRoll>> GetCoaterSewinQueue();
    Task<bool> GetIsSewinQueueChanged(int rowCount, string firstRollNo, string lastRollNo);
    Task<(string styleName, string colorName)> GetNamesFromLegacyCodes(string styleCode, string colorCode);
    Task BasUpdateDefaultRecipe(string styleCode, string rollNo, string recipeName);
    Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence);
    Task SendEmail(string pRecipients, string pSubject, string pBody);
    Task<decimal?> GetCutRollFromHost();
  }
}