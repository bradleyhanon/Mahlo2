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
using Mahlo.Models;

namespace Mahlo.Repository
{
  class DbLocal : IDbLocal
  {
    private IDbConnection connection;

    public DbLocal(IDbConnectionFactoryFactory factoryFactory)
    {
      this.ConnectionFactory = factoryFactory.Create("DbLocal");
    }

    public IDbConnectionFactory ConnectionFactory { get; }

    private IDbConnection Connection
    {
      get
      {
        if (this.connection == null)
        {
          this.connection = ConnectionFactory.GetOpenConnection();
          this.connection.Open();
        }

        return this.connection;
      }
    }

    public void AddGreigeRoll(GreigeRoll roll)
    {
      
    }

    public void DeleteGreigeRoll(GreigeRoll roll)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<GreigeRoll> GetGreigeRolls()
    {
      using (connection = this.ConnectionFactory.GetOpenConnection())
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
      using (connection = this.ConnectionFactory.GetOpenConnection())
      {
        return connection.GetAll<T>();
      }
    }
  }
}
