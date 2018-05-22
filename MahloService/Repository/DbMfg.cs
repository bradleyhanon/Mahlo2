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
  class DbMfg : IDbMfg
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

    public async Task<bool> GetIsSewinQueueChanged(int rowCount, string firstRollNo, string lastRollNo)
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
        return status != 0;
      }
    }

    public async Task<IEnumerable<CarpetRoll>> GetCoaterSewinQueue()
    {
      var p = new DynamicParameters();
      p.Add("Application", "BowAndSkew");

      using (var connection = this.GetOpenConnection())
      {
        //var rolls = await connection.QueryAsync<AS400SewinQueueRoll>("spGetCoaterSewinQueueV2", commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
        var rolls = await connection.QueryAsync<AS400SewinQueueRoll>("spGetSewinQueue", p, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
        return rolls.Select(roll => roll.ToCarpetRoll());
      }
    }

    public async Task<AS400FinishedRoll> GetCutRollFromHost()
    {
      // This reads the current cut roll number from the AS/400
      using (var connection = this.GetOpenConnection())
      {
        //var cmd = connection.CreateCommand();
        //cmd.CommandText = "spGetNextCoaterRoll";
        //cmd.CommandType = CommandType.StoredProcedure;
        //var reader = await cmd.ExecuteReaderAsync();
        //for (int j = 0; j < reader.FieldCount; j++)
        //{
        //  string name = reader.GetName(j);
        //  Type type = reader.GetFieldType(j);
        //  //object value = reader.GetValue(j);
        //  x++;
        //}

        var roll = await connection.QuerySingleAsync<AS400FinishedRoll>("spGetNextCoaterRoll", commandType: CommandType.StoredProcedure);
        return roll;
      }
    }

    // TODO: SaveRollMap needs to be done when we know what Mfg DB can take
    //public Task SaveRollMap(CutRollDetail record)
    //{
    //  var p = new DynamicParameters();
    //  p.Add("CutRoll", size: 8);
    //  p.Add("DatTime");
    //  p.Add("SpecPatternRepeat");
    //  p.Add("MeasuredPatternRepeat");
    //  p.Add("PositionInRoll");
    //}

    public async Task<(string styleName, string colorName)> GetNamesFromLegacyCodes(string styleCode, string colorCode)
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

    public async Task BasUpdateDefaultRecipe(string styleCode, string rollNo, string recipeName)
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

    public async Task<IEnumerable<CoaterScheduleRoll>> GetCoaterSchedule(int minSequence, int maxSequence)
    {
      var p = new DynamicParameters();
      p.Add("MinSequence", minSequence);
      p.Add("MaxSequence", maxSequence);

      using (var connection = this.GetOpenConnection())
      {
        return await connection.QueryAsync<CoaterScheduleRoll>("spGetCoaterSchedule", p, commandType: CommandType.StoredProcedure, commandTimeout: CommandTimeout);
      }
    }

    public async Task SendEmail(string pRecipients, string pSubject, string pBody)
    {
      if ((pRecipients == "" || pBody == ""))
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