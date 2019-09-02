using System;
using System.Threading.Tasks;
using LazyCache;
using Moneteer.Domain.Entities;

namespace Moneteer.Backend.Caching
{
    public class SubscriptionStatusCache
    {
        private readonly IAppCache _memoryCache;

        public SubscriptionStatusCache(IAppCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<SubscriptionStatus> Get(Guid userId)
        {
            var cacheKey = GetCacheKey(userId);

            return _memoryCache.GetAsync<SubscriptionStatus>(cacheKey);
        }

        public void Set(Guid userId, SubscriptionStatus status)
        {
            var cacheKey = GetCacheKey(userId);
            _memoryCache.Add(cacheKey, status, DateTime.UtcNow.AddSeconds(20));
        }

        private string GetCacheKey(Guid userId)
        {
            if (userId == Guid.Empty) throw new InvalidOperationException("Empty user ids are not accepted");

            return CacheKeys.SubscriptionStatus + userId;
        }

        private class CacheKeys
        {
            public static string SubscriptionStatus = "cache://moneteer/subscriptionstatus/";
        }
    }
}