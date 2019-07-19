using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security;
using System.Security.Claims;

namespace Moneteer.Backend.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid GetUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context?.User?.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                throw new SecurityException("Access denied");
            }

            var userId = context.User?.Claims?.SingleOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;

            return Guid.Parse(userId);
        }
    }
}
