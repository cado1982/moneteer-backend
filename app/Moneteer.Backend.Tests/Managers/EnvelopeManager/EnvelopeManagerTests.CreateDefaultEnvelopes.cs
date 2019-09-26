using Moneteer.Domain.Exceptions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class EnvelopeManagerTests : ManagerTests
    {
        [Fact]
        public async Task CreateDefaultEnvelopes_GuardsBudget()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<ForbiddenException>(() => _sut.CreateDefaultEnvelopes(BudgetId, UserId));
        }

        [Fact]
        public async Task CreateDefaultEnvelopes_CallsEnvelopeRepository()
        {
            await _sut.CreateDefaultEnvelopes(BudgetId, UserId);

            Mock.Get(EnvelopeRepository).Verify(r => r.CreateDefaultForBudget(BudgetId, DbConnection), Times.Once);
        }
    }
}
