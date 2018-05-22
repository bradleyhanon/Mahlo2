using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahloService.Repository
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

    public DbConnection GetOpenConnection()
    {
      DbConnection connection = this.ProviderFactory.CreateConnection();
      connection.ConnectionString = this.ConnectionString;
      connection.Open();
      return connection;
    }

    public async Task<DbConnection> GetOpenConnectionAsync()
    {
      DbConnection connection = this.ProviderFactory.CreateConnection();
      connection.ConnectionString = this.ConnectionString;
      await connection.OpenAsync();
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
