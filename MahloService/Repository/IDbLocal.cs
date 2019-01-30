using System.Collections.Generic;
using MahloService.Models;

namespace MahloService.Repository
{
  internal interface IDbLocal
  {
    IDbConnectionFactory ConnectionFactory { get; }

    IEnumerable<GreigeRoll> GetIncompleteGreigeRolls();

    int GetNextGreigeRollId();

    void AddGreigeRoll(GreigeRoll roll);

    void UpdateGreigeRoll(params GreigeRoll[] rolls);

    void DeleteGreigeRoll(GreigeRoll roll);

    string GetProgramState();

    void SetGreigeRollsComplete(IEnumerable<GreigeRoll> rolls);

    long GetLastFootCounterMapped(string tableName);
    int GetNextCutRollId();
    void AddCutRoll(CutRoll cutRoll);
    void UpdateCutRoll(CutRoll cutRoll);
    (double bow, double skew) GetAverageBowAndSkew(long feetCounterStart, long feetCounterEnd);
    double GetAverageElongation(long feetCounterStart, long FeetCounterEnd);
    void InsertMahlo2MapDatum(Mahlo2MapDatum datum);
    void InsertBowAndSkewMapDatum(BowAndSkewMapDatum datum);
    void InsertPatternRepeatMapDatum(PatternRepeatMapDatum datum);
    IEnumerable<CutRoll> GetCutRollsFor(int greigeRollId);
    CutRoll GetCutRoll(int cutRollId);
  }
}