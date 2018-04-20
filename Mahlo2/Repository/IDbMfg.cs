using System.Collections.Generic;
using System.Threading.Tasks;
using Mahlo.Models;

namespace Mahlo.Repository
{
  public interface IDbMfg
  {
    Task<IEnumerable<CarpetRoll>> GetCoaterSewinQueue();
    Task<bool> GetIsSewinQueueChanged(int rowCount, string firstRollNo, string lastRollNo);
    Task<(string styleName, string colorName)> GetNamesFromLegacyCodes(string styleCode, string colorCode);
    Task SendEmail(string pRecipients, string pSubject, string pBody);
  }
}