using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Entities;
using Dapper;
using Moneteer.Domain.Sql;
using System.Data;
using Npgsql;

namespace Moneteer.Domain.Repositories
{
    public class SubscriptionRepository : BaseRepository<SubscriptionRepository>, ISubscriptionRepository
    {
        public SubscriptionRepository(ILogger<SubscriptionRepository> logger)
            : base(logger)
        {
        }

        public async Task<SubscriptionStatus> GetSubscriptionStatus(Guid userId, IDbConnection connection)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@UserId", userId);

                var result = await connection.QuerySingleOrDefaultAsync<SubscriptionStatus>(SubscriptionSql.GetSubscriptionStatus, parameters).ConfigureAwait(false);

                if (result == null) throw new InvalidOperationException("User not found");

                return result;
            }
            catch (PostgresException ex)
            {
                LogPostgresException(ex, $"Error getting subscription status for user: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error getting subscription status for user: {userId}");
                throw;
            }
        }
    }
}