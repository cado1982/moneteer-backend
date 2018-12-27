using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class EnvelopeManagerTests : ManagerTests
    {
        [Fact]
        public async Task GetCategories_GuardsBudget()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.GetEnvelopes(BudgetId, UserId));
        }

        [Fact]
        public async Task GetCategories_CallsRepository()
        {
            await _sut.GetEnvelopes(BudgetId, UserId);

            Mock.Get(EnvelopeRepository).Verify(r => r.GetBudgetEnvelopes(BudgetId, DbConnection), Times.Once);
        }
    }
}
