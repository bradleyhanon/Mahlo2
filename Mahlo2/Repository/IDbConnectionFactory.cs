using System.Data;
using System.Data.Common;

namespace Mahlo.Repository
{
  interface IDbConnectionFactory
  {
    string ConnectionString { get; }
    string ProviderName { get; }
    DbProviderFactory ProviderFactory { get; }

    IDbConnection GetOpenConnection();
  }
}