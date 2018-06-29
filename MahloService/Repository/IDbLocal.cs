using System.Collections.Generic;
using MahloService.Models;

namespace MahloService.Repository
{
  interface IDbLocal
  {
    IDbConnectionFactory ConnectionFactory { get; }

    IEnumerable<GreigeRoll> GetGreigeRolls();

    void AddGreigeRoll(GreigeRoll roll);

    void UpdateGreigeRoll(GreigeRoll roll);

    void DeleteGreigeRoll(GreigeRoll roll);

    string GetProgramState();

    void SetGreigeRollsComplete(IEnumerable<GreigeRoll> rolls);

    int GetNextCutRollId();
    void AddCutRoll(CutRoll cutRoll);
    void UpdateCutRoll(CutRoll cutRoll);
    (double maxBow, double maxSkew) GetBowAndSkew(int greigeRollId, int feetCounterStart, int feetCounterEnd);
  }
}