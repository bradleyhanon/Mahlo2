using System.Collections.Generic;
using MahloService.Models;

namespace MahloService.Repository
{
  interface IDbLocal
  {
    IDbConnectionFactory ConnectionFactory { get; }

    IEnumerable<GreigeRoll> GetIncompleteGreigeRolls();

    int GetNextGreigeRollId();

    void AddGreigeRoll(GreigeRoll roll);

    void UpdateGreigeRoll(GreigeRoll roll);

    void DeleteGreigeRoll(GreigeRoll roll);

    string GetProgramState();

    void SetGreigeRollsComplete(IEnumerable<GreigeRoll> rolls);

    long GetLastFootCounterMapped(string tableName);
    int GetNextCutRollId();
    void AddCutRoll(CutRoll cutRoll);
    void UpdateCutRoll(CutRoll cutRoll);
    (double bow, double skew) GetAverageBowAndSkew(int greigeRollId, long feetCounterStart, long feetCounterEnd);

    void InsertMahlo2MapDatum(Mahlo2MapDatum datum);
    void InsertBowAndSkewMapDatum(BowAndSkewMapDatum datum);
    void InsertPatternRepeatMapDatum(PatternRepeatMapDatum datum);
    IEnumerable<CutRoll> GetCutRollsFor(int greigeRollId);
  }
}