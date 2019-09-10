using System;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.Extensions.Logging;
using Moneteer.Domain.Entities;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;

namespace Moneteer.Backend.Managers
{
    public class SubscriptionManager : BaseManager, ISubscriptionManager
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IAppCache _cache;
        private readonly ILogger<SubscriptionManager> _logger;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(1);

        public SubscriptionManager(
            IConnectionProvider connectionProvider,
            ISubscriptionRepository subscriptionRepository,
            IAppCache cache,
            ILogger<SubscriptionManager> logger,
            Guards guards
        ) : base(guards)
        {
            _connectionProvider = connectionProvider;
            _subscriptionRepository = subscriptionRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<SubscriptionStatus> GetSubscriptionStatus(Guid userId)
        {
            Func<Task<SubscriptionStatus>> statusGetter = async () =>
            {
                _logger.LogDebug("Getting subscription status from database");
                using (var conn = _connectionProvider.GetOpenConnection())
                {
                    return await _subscriptionRepository.GetSubscriptionStatus(userId, conn);
                }
            };

            var cacheKey = GetCacheKey(userId);

            var subscriptionStatus = await _cache.GetOrAdd(cacheKey, statusGetter, DateTime.Now.Add(_cacheExpiry));

            return subscriptionStatus;
        }

        private string GetCacheKey(Guid userId)
        {
            if (userId == Guid.Empty) throw new InvalidOperationException("Empty user ids are not accepted");

            return "cache://moneteer/subscriptionstatus/" + userId;
        }
    }
}