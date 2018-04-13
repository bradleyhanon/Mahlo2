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
using Mahlo.Models;

namespace Mahlo.Repository
{
  class DbLocal : IDbLocal, IProgramStateProvider
  {
    //private IDbConnection connection;

    public DbLocal(IDbConnectionFactoryFactory factoryFactory)
    {
      this.ConnectionFactory = factoryFactory.Create("DbLocal");
    }

    public IDbConnectionFactory ConnectionFactory { get; }

    //private IDbConnection Connection
    //{
    //  get
    //  {
    //    if (this.connection == null)
    //    {
    //      this.connection = ConnectionFactory.GetOpenConnection();
    //    }

    //    return this.connection;
    //  }
    //}

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
        return connection.GetAll<GreigeRoll>().OrderBy(item => item.RollId);
      }
    }

    public void UpdateGreigeRoll(GreigeRoll roll)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<T> GetRolls<T>() where T : MahloRoll
    {
      using (var connection = this.GetOpenConnection())
      {
        return connection.GetAll<T>();
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
        connection.Execute("UPDATE ProgramState SET Value = @Value WHERE Key = 0", new { Value = programState });
      }
    }

    private IDbConnection GetOpenConnection()
    {
      return this.ConnectionFactory.GetOpenConnection();
    }
  }
}