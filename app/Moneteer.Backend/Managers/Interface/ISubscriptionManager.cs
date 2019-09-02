using System;
using System.Threading.Tasks;
using Moneteer.Domain.Entities;

namespace Moneteer.Backend.Managers
{
    public interface ISubscriptionManager
    {
        Task<SubscriptionStatus> GetSubscriptionStatus(Guid userId);
    }
}