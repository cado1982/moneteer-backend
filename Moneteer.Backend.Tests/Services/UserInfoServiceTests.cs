using IdentityModel;
using Microsoft.AspNetCore.Http;
using Moneteer.Backend.Services;
using Moq;
using System;
using System.Security;
using System.Security.Claims;
using Xunit;

namespace Moneteer.Backend.Tests.Services
{
    public class UserInfoServiceTests
    {
        private readonly UserInfoService _sut;

        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new Mock<IHttpContextAccessor>();

        public UserInfoServiceTests()
        {
            _sut = new UserInfoService(_httpContextAccessor.Object);
        }

        [Fact]
        public void GetUserId_WhenUserNull_Throws()
        {
            _httpContextAccessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());

            Assert.Throws<SecurityException>(() => _sut.GetUserId());
        }

        [Fact]
        public void GetUserId_WhenIdentityNull_Throws()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal();

            _httpContextAccessor.Setup(a => a.HttpContext).Returns(context);

            Assert.Throws<SecurityException>(() => _sut.GetUserId());
        }

        [Fact]
        public void GetUserId_WhenNotAuthenticated_Throws()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity());

            _httpContextAccessor.Setup(a => a.HttpContext).Returns(context);

            Assert.Throws<SecurityException>(() => _sut.GetUserId());
        }

        [Fact]
        public void GetUserId_HappyPath()
        {
            var expectedUserId = Guid.NewGuid();

            var context = new DefaultHttpContext();
            var identity = new ClaimsIdentity("MYAUTH");
            identity.AddClaim(new Claim(JwtClaimTypes.Subject, expectedUserId.ToString()));
            context.User = new ClaimsPrincipal(identity);

            _httpContextAccessor.Setup(a => a.HttpContext).Returns(context);

            var actualUserId = _sut.GetUserId();

            Assert.Equal(expectedUserId, actualUserId);
        }
    }
}
