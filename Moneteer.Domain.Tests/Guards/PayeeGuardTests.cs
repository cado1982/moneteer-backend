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
    public class PayeeGuardTests
    {
        private Mock<IPayeeRepository> _payeeRepositoryMock;
        private Mock<IConnectionProvider> _connectionProviderMock;
        private PayeeGuard _sut;

        private Guid _payeeId = Guid.NewGuid();
        private Guid _payeeOwnerId = Guid.NewGuid();

        public PayeeGuardTests()
        {
            _payeeRepositoryMock = new Mock<IPayeeRepository>();
            _connectionProviderMock = new Mock<IConnectionProvider>();

            _payeeRepositoryMock.Setup(r => r.GetOwner(_payeeId, It.IsAny<IDbConnection>())).ReturnsAsync(_payeeOwnerId);

            _sut = new PayeeGuard(_payeeRepositoryMock.Object, _connectionProviderMock.Object);
        }

        [Fact]
        public Task Blocks()
        {
            return Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.Guard(_payeeId, Guid.NewGuid()));
        }

        [Fact]
        public Task Allows()
        {
            return _sut.Guard(_payeeId, _payeeOwnerId);
        }
    }
}
