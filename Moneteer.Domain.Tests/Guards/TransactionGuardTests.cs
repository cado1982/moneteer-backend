using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Domain.Tests.Guards
{
    public class TransactionGuardTests
    {
        private Mock<ITransactionRepository> _transactionRepositoryMock;
        private Mock<IConnectionProvider> _connectionProviderMock;
        private TransactionGuard _sut;

        private Guid _transactionId = Guid.NewGuid();
        private Guid _transactionOwnerId = Guid.NewGuid();

        public TransactionGuardTests()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _connectionProviderMock = new Mock<IConnectionProvider>();

            _transactionRepositoryMock.Setup(r => r.GetOwners(new List<Guid> { _transactionId }, It.IsAny<IDbConnection>())).ReturnsAsync(new List<Guid>{_transactionOwnerId});

            _sut = new TransactionGuard(_transactionRepositoryMock.Object, _connectionProviderMock.Object);
        }

        [Fact]
        public Task Blocks()
        {
            return Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.Guard(_transactionId, Guid.NewGuid()));
        }

        [Fact]
        public Task Allows()
        {
            return _sut.Guard(_transactionId, _transactionOwnerId);
        }
    }
}
