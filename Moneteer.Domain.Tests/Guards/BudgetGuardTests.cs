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
    public class BudgetGuardTests
    {
        private Mock<IBudgetRepository> _budgetRepositoryMock;
        private Mock<IDbConnection> _connectionMock;
        private BudgetGuard _sut;

        private Guid _budgetId = Guid.NewGuid();
        private Guid _budgetOwnerId = Guid.NewGuid();

        public BudgetGuardTests()
        {
            _budgetRepositoryMock = new Mock<IBudgetRepository>();
            _connectionMock = new Mock<IDbConnection>();

            _budgetRepositoryMock.Setup(r => r.GetOwner(_budgetId, It.IsAny<IDbConnection>())).ReturnsAsync(_budgetOwnerId);

            _sut = new BudgetGuard(_budgetRepositoryMock.Object);
        }

        [Fact]
        public Task Blocks()
        {
            return Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.Guard(_budgetId, Guid.NewGuid(), _connectionMock.Object));
        }

        [Fact]
        public Task Allows()
        {
            return _sut.Guard(_budgetId, _budgetOwnerId, _connectionMock.Object);
        }
    }
}
