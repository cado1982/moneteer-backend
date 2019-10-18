using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Moneteer.Backend.Handlers
{
    public class SubscriptionMiddleware
    {
        private readonly RequestDelegate _next;
        public SubscriptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context, ILogger<SubscriptionMiddleware> logger)
        {
            logger.LogTrace("Entered InvokeAsync");

            if (context.Request.Path == "/healthcheck" || context.Request.Path == "/favicon.ico")
            {
                await _next(context);
                return;
            }

            var user = context.User;
            Claim sub = null;

            if (user == null || (sub = user.FindFirst(c => c.Type == JwtClaimTypes.Subject)) == null)
            {
                logger.LogWarning("User not found. Setting response code to 401");
                context.Response.StatusCode = 401;
                return;
            }

            var userId = sub.Value;
            var unixTimeNow = DateTime.UtcNow.Subtract(new DateTime(1970,1,1)).TotalSeconds;

            var trialExpiryClaim = user.Claims.Single(c => c.Type == ClaimTypes.TrialExpiry);
            var subscriptionExpiryClaim = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.SubscriptionExpiry);

            var trialExpiry = Int32.Parse(trialExpiryClaim.Value);
            logger.LogTrace($"Found trial expiry claim for user {userId} with value {trialExpiry}");

            int? subscriptionExpiry = null;
            if (subscriptionExpiryClaim != null)
            {
                subscriptionExpiry = Int32.Parse(subscriptionExpiryClaim.Value);
                logger.LogTrace($"Found subscription expiry claim for user {userId} with value {subscriptionExpiry}");
            }

            if (trialExpiry > unixTimeNow ||
               (subscriptionExpiry != null && subscriptionExpiry > unixTimeNow))
            {
                logger.LogTrace($"Subscription or trial active for user {userId}");
                await _next(context);
            }
            else 
            {
                logger.LogTrace($"Subscription or trial expired for user {userId}");
                context.Response.StatusCode = 402;
            }
        }

        private static class ClaimTypes 
        {
            public static string SubscriptionExpiry = "subscription_expiry";
            public static string TrialExpiry = "trial_expiry";
        }
    }
}