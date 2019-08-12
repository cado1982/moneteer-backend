using Moneteer.Models;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class EnvelopeManagerTests : ManagerTests
    {
        [Fact]
        public async Task CreateEnvelopeCategory_GuardsBudget()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.CreateEnvelopeCategory(BudgetId, new EnvelopeCategory { Name = "I am an envelope category" }, UserId));
        }

        [Fact]
        public async Task CreateEnvelopeCategory_CallsRepository()
        {
            var model = new EnvelopeCategory
            {
                Name = "MyCategory"
            };

            await _sut.CreateEnvelopeCategory(BudgetId, model, UserId);

            Mock.Get(EnvelopeRepository).Verify(r => r.CreateEnvelopeCategory(BudgetId, It.Is<Domain.Entities.EnvelopeCategory>(cc => cc.Name == "MyCategory"), DbConnection), Times.Once);
        }
    }
}
