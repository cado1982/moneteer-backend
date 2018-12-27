using System.Data;

namespace Moneteer.Domain.Helpers
{
    public interface IConnectionProvider
    {
        IDbConnection GetConnection();
        IDbConnection GetOpenConnection();
    }
}