using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.FluentColumnMapping;
using MahloService.Models;

namespace MahloService.Repository
{
  internal class DbMfg : IDbMfg
  {
    private const int CommandTimeout = 10;

    static DbMfg()
    {
      // Create a new mapping collection
      var mappings = new ColumnMappingCollection();

      // Defining the mappings between each property/column for a type
      mappings.RegisterType<AS400SewinQueueRoll>()
              .MapProperty(x => x.G2SCH).ToColumn("G2SCH#");

      mappings.RegisterType<CoaterScheduleRoll>()
        .MapProperty(x => x.SeqNo).ToColumn("Seq #")
        .MapProperty(x => x.SchedNo).ToColumn("Sched #")
        .MapProperty(x => x.CutLength).ToColumn("Cut Length")
        .MapProperty(x => x.Sewnin).ToColumn("Sewn-in")
        .MapProperty(x => x.FaceWt).ToColumn("Face Wt")
        .MapProperty(x => x.OrigSeq).ToColumn("Orig Seq");

      // Tell Dapper to use our custom mappings
      mappings.RegisterWithDapper();
    }

    public DbMfg(IDbConnectionFactoryFactory factoryFactory)
    {
      this.ConnectionFactory = factoryFactory.Create("DbMfg");
    }

    public IDbConnectionFactory ConnectionFactory { get; }

    public async Task<bool> GetIsSewinQueueChangedAsync(int rowCount, string firstRollNo, string lastRollNo)
    {
      var p = new DynamicParameters();
      p.Add("status", 0, direction: ParameterDirection.InputOutput);
      p.Add("process", 1);
      p.Add("queuesize", rowCount);
      p.Add("first_roll", firstRollNo);
      p.Add("last_roll", lastRollNo);

      using (var connection = this.GetOpenConnection())
      {
        await connection.ExecuteAsync("spSewinQueueChanged", p, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
        int status = p.Get<int>("status");
        bool result = status != 0;
        //Console.WriteLine($"queuesize={rowCount}, first_roll={firstRollNo}, last_Roll={lastRollNo}, status={status}, result={result}");
        return result;
      }
    }

    public async Task<IEnumerable<GreigeRoll>> GetCoaterSewinQueueAsync()
    {
      var p = new DynamicParameters();
      p.Add("Application", "BowAndSkew");

      using (var connection = this.GetOpenConnection())
      {
        //var rolls = await connection.QueryAsync<AS400SewinQueueRoll>("spGetCoaterSewinQueueV2", commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
        var rolls = await connection.QueryAsync<AS400SewinQueueRoll>("spGetSewinQueue", p, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
        return rolls.Select(roll => roll.ToGreigeRoll());
      }
    }

    /// <summary>
    /// Read the current cut roll number from the AS/400
    /// </summary>
    /// <returns>The current cut roll number or null if there isn't one.</returns>
    public async Task<decimal?> GetCutRollFromHostAsync()
    {
      // This reads the current cut roll number from the AS/400
      try
      {
        using (var connection = this.GetOpenConnection())
        {
          var roll = await connection.QuerySingleAsync<AS400CutRoll>("spGetNextCoaterRoll", commandType: CommandType.StoredProcedure);
          return roll?.FRCROL;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<(string styleName, string colorName)> GetNamesFromLegacyCodesAsync(string styleCode, string colorCode)
    {
      var p = new DynamicParameters();
      p.Add("style_code", styleCode);
      p.Add("color_code", colorCode);
      p.Add("style_name", string.Empty, DbType.AnsiString, ParameterDirection.Output);
      p.Add("color_name", string.Empty, DbType.AnsiString, ParameterDirection.Output);

      using (var connection = this.GetOpenConnection())
      {
        await connection.ExecuteAsync("spGetNamesFromLegacyCodes", commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
        var result = (p.Get<string>("style_name"), p.Get<string>("color_name"));
        return result;
      }
    }

    public async Task BasUpdateDefaultRecipeAsync(string styleCode, string rollNo, string recipeName)
    {
      var p = new DynamicParameters();
      p.Add("Application", "BowAndSkew");
      p.Add("StyleCode", styleCode);
      p.Add("RollNumber", rollNo);
      p.Add("RecipeName", recipeName);

      try
      {
        using (var connection = this.GetOpenConnection())
        {
          await connection.ExecuteAsync("spUpdateCoaterMahloDefaultRecipe", p, commandType: CommandType.StoredProcedure, commandTimeout: 5);
        }
      }
      catch
      {
      }
    }

    public async Task<IEnumerable<CoaterScheduleRoll>> GetCoaterScheduleAsync(int minSequence, int maxSequence)
    {
      var p = new DynamicParameters();
      p.Add("MinSequence", minSequence);
      p.Add("MaxSequence", maxSequence);

      using (var connection = this.GetOpenConnection())
      {
        return await connection.QueryAsync<CoaterScheduleRoll>("spGetCoaterSchedule", p, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
      }
    }

    public async Task SendEmailAsync(string pRecipients, string pSubject, string pBody)
    {
      if ((string.IsNullOrEmpty(pRecipients) || string.IsNullOrEmpty(pBody)))
      {
        return;
      }

      var p = new DynamicParameters();
      p.Add("Result", dbType: DbType.Int32, direction: ParameterDirection.Output);
      p.Add("BodyFormat", "TextBody", size: 10);
      p.Add("From", "mahlo_bow_bias@mannington.com", size: 100);
      p.Add("Recipients", pRecipients, size: 200);
      p.Add("Subject", pSubject.Substring(0, Math.Min(100, pSubject.Length)), size: 100);
      p.Add("Body", pBody.Substring(0, Math.Min(4000, pBody.Length)), size: 4000);

      try
      {
        using (var connection = this.GetOpenConnection())
        {
          await connection.ExecuteAsync("sp_send_cdosysmail", p, commandType: CommandType.StoredProcedure, commandTimeout: 10);
          p.Get<int>("Result");
        }
      }
      catch
      {
      }
    }

    private DbConnection GetOpenConnection()
    {
      return this.ConnectionFactory.GetOpenConnection();
    }
  }
}