using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Domain.Tests.Guards
{
    public class AccountGuardTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private Mock<IConnectionProvider> _connectionProviderMock;
        private AccountGuard _sut;

        private Guid _accountId = Guid.NewGuid();
        private Guid _accountOwnerId = Guid.NewGuid();

        public AccountGuardTests()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>();
            _connectionProviderMock = new Mock<IConnectionProvider>();

            _accountRepositoryMock.Setup(r => r.GetOwner(_accountId, It.IsAny<IDbConnection>())).ReturnsAsync(_accountOwnerId);
            
            _sut = new AccountGuard(_accountRepositoryMock.Object, _connectionProviderMock.Object);
        }

        [Fact]
        public Task Blocks()
        {
            return Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.Guard(_accountId, Guid.NewGuid()));
        }

        [Fact]
        public Task Allows()
        {
            return _sut.Guard(_accountId, _accountOwnerId);
        }
    }
}
