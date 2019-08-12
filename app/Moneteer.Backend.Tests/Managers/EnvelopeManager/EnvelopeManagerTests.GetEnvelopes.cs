using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class EnvelopeManagerTests : ManagerTests
    {
        [Fact]
        public async Task GetEnvelopes_GuardsBudget()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.GetEnvelopes(BudgetId, UserId));
        }

        [Fact]
        public async Task GetEnvelopes_CallsRepository()
        {
            await _sut.GetEnvelopes(BudgetId, UserId);

            Mock.Get(EnvelopeRepository).Verify(r => r.GetBudgetEnvelopes(BudgetId, DbConnection), Times.Once);
        }

        [Fact]
        public async Task GetEnvelopes_CalculatesBalances()
        {
            var result = await _sut.GetEnvelopes(BudgetId, UserId);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(50, result.First().Balance);
        } 
    }
}
