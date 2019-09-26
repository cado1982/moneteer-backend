using Moneteer.Domain.Exceptions;
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
        private Mock<IDbConnection> _connectionMock;
        private PayeeGuard _sut;

        private Guid _payeeId = Guid.NewGuid();
        private Guid _payeeOwnerId = Guid.NewGuid();

        public PayeeGuardTests()
        {
            _payeeRepositoryMock = new Mock<IPayeeRepository>();
            _connectionMock = new Mock<IDbConnection>();

            _payeeRepositoryMock.Setup(r => r.GetOwner(_payeeId, It.IsAny<IDbConnection>())).ReturnsAsync(_payeeOwnerId);

            _sut = new PayeeGuard(_payeeRepositoryMock.Object);
        }

        [Fact]
        public Task Blocks()
        {
            return Assert.ThrowsAsync<ForbiddenException>(() => _sut.Guard(_payeeId, Guid.NewGuid(), _connectionMock.Object));
        }

        [Fact]
        public Task Allows()
        {
            return _sut.Guard(_payeeId, _payeeOwnerId, _connectionMock.Object);
        }
    }
}
