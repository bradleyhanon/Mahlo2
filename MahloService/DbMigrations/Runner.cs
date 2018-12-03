using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using MahloService.Repository;

namespace MahloService.DbMigrations
{
  internal class Runner
  {
    private readonly IDbLocal dbLocal;

    public Runner(IDbLocal dbLocal)
    {
      this.dbLocal = dbLocal;
    }

    public void MigrateToLatest()
    {
      var providerMap = new[] {
        new { Name = "SqlServer", Provider = "System.Data.SqlClient" },
        new { Name = "MySql", Provider = "MySql.Data.MySqlClient" },
        new { Name = "SQLite", Provider = "System.Data.SQLite" },
        new { Name = "Postgres", Provider = "Npgsql" },
        new { Name = "Firebird", Provider = "FirebirdSql.Data.FirebirdClient" },
        new { Name = "SqlServerCe", Provider = "System.Data.SqlServerCe.4.0" },
        new { Name = "Jet", Provider = "Microsoft.Jet.OLEDB.4.0" },
        new { Name = "Hana", Provider = "Sap.Data.Hana" },
        new { Name = "Db2", Provider = "IBM.Data.DB2" },
      };

      // Unmapped names that FluentMigrator supports but the provider invariant name isn't known
      // DotConnectOracle, 
      // new { Name = "Oracle", Provider = "System.Data.OracleClient" },
      // OracleManaged, 
      // SqlServer2000, 
      // SqlServer2005, 
      // SqlServer2008, 
      // SqlServer2012, 
      // SqlServer2014, 


      //ConnectionStringSettings css = ConfigurationManager.ConnectionStrings["DbLocal"];
      this.CreateDatabaseIfNeeded();

      string migrationFactoryName = providerMap.Single(item => item.Provider == this.dbLocal.ConnectionFactory.ProviderName).Name;

      var options = new MigrationOptions();
      var announcer = new FluentMigrator.Runner.Announcers.TextWriterAnnouncer(s => Console.WriteLine(s));
      //var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServerProcessorFactory();
      var factory = new FluentMigrator.Runner.Processors.MigrationProcessorFactoryProvider().GetFactory(migrationFactoryName);
      var context = new FluentMigrator.Runner.Initialization.RunnerContext(announcer)
      {
        Namespace = "MahloService.DbMigrations",
        NestedNamespaces = false,
      };

      var processor = factory.Create(this.dbLocal.ConnectionFactory.ConnectionString, announcer, options);
      var runner = new FluentMigrator.Runner.MigrationRunner(Assembly.GetExecutingAssembly(), context, processor);
      runner.MigrateUp();
    }

    private void CreateDatabaseIfNeeded()
    {
      DbConnectionStringBuilder csb = this.dbLocal.ConnectionFactory.ProviderFactory.CreateConnectionStringBuilder();
      csb.ConnectionString = this.dbLocal.ConnectionFactory.ConnectionString;
      string databaseName = (string)csb["Initial Catalog"];
      csb["Initial Catalog"] = string.Empty;

      using (IDbConnection connection = this.dbLocal.ConnectionFactory.ProviderFactory.CreateConnection())
      {
        connection.ConnectionString = csb.ToString();
        connection.Open();
        using (IDbCommand command = connection.CreateCommand())
        {
          try
          {
            command.CommandText = $"CREATE DATABASE {databaseName}";
            command.ExecuteNonQuery();
          }
          catch (DbException)
          {
          }
        }
      }
    }

    private class MigrationOptions : IMigrationProcessorOptions
    {
      public bool PreviewOnly { get; set; } = false;
      public int? Timeout { get; set; } = 60;
      public string ProviderSwitches { get; set; } = string.Empty;
    }
  }
}
