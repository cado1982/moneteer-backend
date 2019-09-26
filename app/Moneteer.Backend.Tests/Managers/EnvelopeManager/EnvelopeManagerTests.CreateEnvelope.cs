using Moneteer.Domain.Exceptions;
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
        public async Task CreateEnvelope_GuardsBudget()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<ForbiddenException>(() => _sut.CreateEnvelope(BudgetId, new Envelope(), UserId));
        }

        [Fact]
        public async Task CreateEnvelope_CallsRepository()
        {
            var envelopeCategoryId = Guid.NewGuid();

            var model = new Envelope
            {
                Name = "MyEnvelope",
                EnvelopeCategory = new EnvelopeCategory
                {
                    Id = envelopeCategoryId
                }
            };

            await _sut.CreateEnvelope(BudgetId, model, UserId);

            Mock.Get(EnvelopeRepository).Verify(r => r.CreateEnvelope(It.Is<Domain.Entities.Envelope>(cc => cc.Name == "MyEnvelope" && cc.EnvelopeCategory.Id == envelopeCategoryId), DbConnection), Times.Once);
        }

    }
}
