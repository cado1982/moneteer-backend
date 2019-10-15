using Moneteer.Domain.Exceptions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Repositories;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Domain.Tests.Guards
{
    public class EnvelopeGuardTests
    {
        private Mock<IEnvelopeRepository> _envelopeRepositoryMock;
        private Mock<IDbConnection> _connectionMock;
        private EnvelopeGuard _sut;

        private Guid _envelopeId = Guid.NewGuid();
        private Guid _envelopeOwnerId = Guid.NewGuid();

        public EnvelopeGuardTests()
        {
            _envelopeRepositoryMock = new Mock<IEnvelopeRepository>();
            _connectionMock = new Mock<IDbConnection>();

            _envelopeRepositoryMock.Setup(r => r.GetEnvelopeOwner(_envelopeId, It.IsAny<IDbConnection>())).ReturnsAsync(_envelopeOwnerId);
            
            _sut = new EnvelopeGuard(_envelopeRepositoryMock.Object);
        }

        [Fact]
        public Task Blocks()
        {
            return Assert.ThrowsAsync<ForbiddenException>(() => _sut.Guard(_envelopeId, Guid.NewGuid(), _connectionMock.Object));
        }

        [Fact]
        public Task Allows()
        {
            return _sut.Guard(_envelopeId, _envelopeOwnerId, _connectionMock.Object);
        }
    }
}
