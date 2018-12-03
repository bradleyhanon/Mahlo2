using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using Dapper.FluentColumnMapping;
using MahloService.Models;

namespace MahloService.Repository
{
  class DbLocal : IDbLocal, IProgramStateProvider
  {
    public DbLocal(IDbConnectionFactoryFactory factoryFactory)
    {
      this.ConnectionFactory = factoryFactory.Create("DbLocal");
    }

    public IDbConnectionFactory ConnectionFactory { get; }

    public void AddGreigeRoll(GreigeRoll roll)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Insert(roll);
      }
    }

    public void DeleteGreigeRoll(GreigeRoll roll)
    {
      throw new NotImplementedException();
    }

    public int GetNextGreigeRollId()
    {
      using (var connection = this.GetOpenConnection())
      {
        return (connection.ExecuteScalar<int?>("SELECT MAX(Id) FROM GreigeRolls") ?? 0) + 1;
      }
    }

    public IEnumerable<GreigeRoll> GetIncompleteGreigeRolls()
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.Query<GreigeRoll>("SELECT * FROM GreigeRolls WHERE IsComplete = 0 ORDER BY Id");
      }
    }

    public void UpdateGreigeRoll(params GreigeRoll[] rolls)
    {
      using (var connection = this.GetOpenConnection())
      using (var transaction = connection.BeginTransaction())
      {
        foreach (var roll in rolls)
        {
          connection.Update(roll, transaction: transaction);
        }

        transaction.Commit();
      }
    }

    public void SetGreigeRollsComplete(IEnumerable<GreigeRoll> rolls)
    {
      using (var connection = this.GetOpenConnection())
      {
        foreach(var roll in rolls)
        {
          roll.IsComplete = true;
          connection.Update(roll);
        }
      }
    }

    public async Task SaveRollAsync<T>(T roll) 
      where T : MahloModel
    {
      using (var connection = this.GetOpenConnection())
      {
        await connection.InsertAsync(roll);
      }
    }

    public void UpdateRoll<T>(T roll)
      where T:MahloModel
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Update(roll);
      }
    }

    public string GetProgramState()
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.QuerySingle<string>("SELECT Value FROM ProgramState");
      }
    }

    public void SaveProgramState(string programState)
    {
      using (var connection = this.GetOpenConnection())
      {
        if (connection.ExecuteScalar<int>("SELECT COUNT(*) FROM ProgramState WHERE [Key] = 0") == 0)
        {
          connection.Execute("INSERT INTO ProgramState([Key], Value) VALUES(0, @Value)", new { Value = programState });
        }
        else
        {
          connection.Execute("UPDATE ProgramState SET Value = @Value WHERE [Key] = 0", new { Value = programState });
        }
      }
    }

    private IDbConnection GetOpenConnection()
    {
      return this.ConnectionFactory.GetOpenConnection();
    }

    public long GetLastFootCounterMapped(string tableName)
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.ExecuteScalar<long?>($"SELECT MAX(FeetCounter) FROM {tableName}") ?? 0;
      }
    }

    public IEnumerable<CutRoll> GetCutRollsFor(int greigeRollId)
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.Query<CutRoll>(
          "SELECT * FROM CutRolls WHERE GreigeRollId = @GreigeRollId ORDER BY Id", 
          new { GreigeRollId = greigeRollId });
      }
    }

    public int GetNextCutRollId()
    {
      using (var connection = this.GetOpenConnection())
      {
        return (connection.ExecuteScalar<int?>("SELECT MAX(Id) FROM CutRolls") ?? 0) + 1;
      }
    }

    public void AddCutRoll(CutRoll cutRoll)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Insert(cutRoll);
      }
    }

    public void UpdateCutRoll(CutRoll cutRoll)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Update(cutRoll);
      }
    }

    public (double bow, double skew) GetAverageBowAndSkew(int greigeRollId, long feetCounterStart, long feetCounterEnd)
    {
      //      @"DECLARE @Id Int = 1
      //DECLARE @StartFeet Int = 100
      //DECLARE @EndFeet Int = 200

      string cmdText =
        @"SELECT COALESCE(AVG(Bow), 0.0) AvgBow, 
                 COALESCE(AVG(Skew), 0.0) AvgSkew
          FROM BowAndSkewMap map
          JOIN (SELECT gr.BasFeetCounterStart + (@StartFeet - gr.PrsFeetCounterStart) BasStart,
		               (SELECT MIN(v)
			            FROM (VALUES (gr.BasFeetCounterStart + (@EndFeet - gr.PrsFeetCounterStart)), (gr.BasFeetCounterEnd)) as value(v)) BasEnd
	            FROM GreigeRolls gr
	            WHERE gr.Id = @Id) sub
          ON sub.BasStart >= map.FeetCounter AND map.FeetCounter < sub.BasEnd";

      using (var connection = this.GetOpenConnection())
      {
        var result = connection.Query(cmdText, new { Id = greigeRollId, StartFeet = feetCounterStart, EndFeet = feetCounterEnd }).First();
        return (result.AvgBow, result.AvgSkew);
      }
    }

    public void InsertMahlo2MapDatum(Mahlo2MapDatum datum)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Insert(datum);
      }
    }

    public void InsertBowAndSkewMapDatum(BowAndSkewMapDatum datum)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Insert(datum);
      }
    }

    public void InsertPatternRepeatMapDatum(PatternRepeatMapDatum datum)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Insert(datum);
      }
    }
  }
}