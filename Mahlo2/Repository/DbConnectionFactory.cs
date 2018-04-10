using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mahlo.Repository
{
  interface IDbConnectionFactoryFactory
  {
    IDbConnectionFactory Create(string dbName);
  }

  class DbConnectionFactory : IDbConnectionFactory
  {
    public DbConnectionFactory(string dbName)
    {
      var settings = ConfigurationManager.ConnectionStrings[dbName];
      this.ConnectionString = settings.ConnectionString;
      this.ProviderName = settings.ProviderName;
      this.ProviderFactory = DbProviderFactories.GetFactory(settings.ProviderName);
    }

    public string ConnectionString { get; }

    public DbProviderFactory ProviderFactory { get; }

    public string ProviderName { get; }

    public IDbConnection GetOpenConnection()
    {
      IDbConnection connection = this.ProviderFactory.CreateConnection();
      connection.ConnectionString = this.ConnectionString;
      connection.Open();
      return connection;
    }

    public class Factory : IDbConnectionFactoryFactory
    {
      public IDbConnectionFactory Create(string dbName)
      {
        return new DbConnectionFactory(dbName);
      }
    }
  }
}
