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

    }

    public void DeleteGreigeRoll(GreigeRoll roll)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<GreigeRoll> GetGreigeRolls()
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.GetAll<GreigeRoll>().OrderBy(item => item.Id);
      }
    }

    public void UpdateGreigeRoll(GreigeRoll roll)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Update(roll);
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

    public void SaveRoll<T>(T roll) 
      where T : MahloRoll
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.InsertAsync(roll);
      }
    }

    public void UpdateRoll<T>(T roll)
      where T:MahloRoll
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
        connection.Execute("UPDATE ProgramState SET Value = @Value WHERE [Key] = 0", new { Value = programState });
      }
    }

    private IDbConnection GetOpenConnection()
    {
      return this.ConnectionFactory.GetOpenConnection();
    }

    public int GetNextCutRollId()
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.ExecuteScalar<int?>("SELECT MAX(Id) FROM GreigeRolls") ?? 1;
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

    public void AddBowAndSkewEntry(int feetCounter, double bow, double skew)
    {
      using (var connection = this.GetOpenConnection())
      {
        string cmdText = "INSERT INTO BowAndSkewMap (FeetCounter, Bow, Skew) VALUES (@FeetCounter, @Bow, @Skew)";
        connection.Execute(cmdText, new { FeetCounter = feetCounter, Bow = bow, Skew = skew });
      }
    }

    public (double maxBow, double maxSkew) GetBowAndSkew(int greigeRollId, int feetCounterStart, int feetCounterEnd)
    {
      //      @"DECLARE @Id Int = 1
      //DECLARE @StartFeet Int = 100
      //DECLARE @EndFeet Int = 200

      string cmdText =
        @"SELECT COALESCE(MIN(Bow), 0.0) MinBow, 
                 COALESCE(MAX(Bow), 0.0) MaxBow, 
                 COALESCE(MIN(Skew), 0.0) MinSkew, 
                 COALESCE(MAX(Skew), 0.0) MaxSkew
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
        return (Math.Abs(result.MinBow) > Math.Abs(result.MaxBow) ? result.MinBow : result.MaxBow,
                Math.Abs(result.MinSkew) > Math.Abs(result.MaxSkew) ? result.MinSkew : result.MaxSkew);
      }
    }
  }
}