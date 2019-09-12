using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Moneteer.Backend.Managers;
using System.Security.Claims;
using IdentityModel;

namespace Moneteer.Backend.Handlers
{
    public class SubscriptionAuthorizationHandler : AuthorizationHandler<SubscriptionAuthorizeRequirement>
    {
        private readonly ISubscriptionManager _subscriptionManager;

        public SubscriptionAuthorizationHandler(ISubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubscriptionAuthorizeRequirement requirement)
        {
            var userId = context.User.FindFirstValue(JwtClaimTypes.Subject);

            if (userId == null) throw new InvalidOperationException($"User claim is not found");

            var userGuid = new Guid(userId);

            var subscriptionStatus = await _subscriptionManager.GetSubscriptionStatus(userGuid);

            if (subscriptionStatus.TrialExpiry > DateTime.UtcNow ||
               (subscriptionStatus.SubscriptionExpiry != null && subscriptionStatus.SubscriptionExpiry > DateTime.UtcNow))
            {
                context.Succeed(requirement);
            }
            else 
            {
                context.Fail();
            }
        }
    }

    public class SubscriptionAuthorizeRequirement: IAuthorizationRequirement
    {

    }
}