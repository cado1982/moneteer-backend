using System;
using System.Data;
using System.Threading.Tasks;
using Moneteer.Domain.Entities;

namespace Moneteer.Domain.Repositories
{
    public interface ISubscriptionRepository
    {
         Task<SubscriptionStatus> GetSubscriptionStatus(Guid userId, IDbConnection connection);
    }
}