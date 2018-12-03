using System.Data.Common;
using System.Threading.Tasks;

namespace MahloService.Repository
{
  internal interface IDbConnectionFactory
  {
    string ConnectionString { get; }
    string ProviderName { get; }
    DbProviderFactory ProviderFactory { get; }

    DbConnection GetOpenConnection();

    Task<DbConnection> GetOpenConnectionAsync();
  }
}