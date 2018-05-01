﻿using System;
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
using Mahlo.Models;

namespace Mahlo.Repository
{
  class DbLocal : IDbLocal, IProgramStateProvider
  {
    public DbLocal(IDbConnectionFactoryFactory factoryFactory)
    {
      this.ConnectionFactory = factoryFactory.Create("DbLocal");
    }

    public IDbConnectionFactory ConnectionFactory { get; }

    public void AddCarpetRoll(CarpetRoll roll)
    {

    }

    public void DeleteCarpetRoll(CarpetRoll roll)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<CarpetRoll> GetCarpetRolls()
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.GetAll<CarpetRoll>().OrderBy(item => item.Id);
      }
    }

    public void UpdateCarpetRoll(CarpetRoll roll)
    {
      using (var connection = this.GetOpenConnection())
      {
        connection.Update(roll);
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
  }
}