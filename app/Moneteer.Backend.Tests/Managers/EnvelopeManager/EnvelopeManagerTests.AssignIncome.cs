using Moneteer.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class EnvelopeManagerTests : ManagerTests
    {
        [Fact]
        public async Task AssignIncome_GuardsBudget()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.AssignIncome(BudgetId, new AssignIncomeRequest(), UserId));
        }

        [Fact]
        public async Task AssignIncome_WhenBudgetIdNull_Fails()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _sut.AssignIncome(Guid.Empty, null, UserId));
        }

        [Fact]
        public async Task AssignIncome_WhenTryingToAssignTooMuchIncome_Fails()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetAvailableIncome(BudgetId, DbConnection)).ReturnsAsync(40);

            var request = new AssignIncomeRequest
            {
                Assignments = new List<AssignIncome>
                {
                    new AssignIncome
                    {
                        Amount = 50,
                        Envelope = new Envelope()
                    }
                }
            };

            await Assert.ThrowsAsync<ApplicationException>(() => _sut.AssignIncome(BudgetId, request, UserId));
        }

        [Fact]
        public async Task AssignIncome_AdjustsEnvelopeAssigned()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetAvailableIncome(BudgetId, DbConnection)).ReturnsAsync(110);

            var request = BuildExampleAssignIncomeRequest();

            await _sut.AssignIncome(BudgetId, request, UserId);

            Mock.Get(EnvelopeRepository).Verify(r => r.AdjustAssigned(request.Assignments[0].Envelope.Id, 50, DbConnection), Times.Once);
            Mock.Get(EnvelopeRepository).Verify(r => r.AdjustAssigned(request.Assignments[1].Envelope.Id, 60, DbConnection), Times.Once);
        }

        [Fact]
        public async Task AssignIncome_AdjustsAvailableIncome()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetAvailableIncome(BudgetId, DbConnection)).ReturnsAsync(110);

            var request = BuildExampleAssignIncomeRequest();

            await _sut.AssignIncome(BudgetId, request, UserId);

            Mock.Get(BudgetRepository).Verify(r => r.AdjustAvailable(BudgetId, -110, DbConnection), Times.Once);
        }

        [Fact]
        public async Task AssignIncome_UsesTransaction()
        {
            var request = new AssignIncomeRequest();

            await _sut.AssignIncome(BudgetId, request, UserId);

            Mock.Get(DbConnection).Verify(c => c.BeginTransaction(), Times.Once);
            Mock.Get(DbTransaction).Verify(t => t.Commit(), Times.Once);
        }

        private AssignIncomeRequest BuildExampleAssignIncomeRequest()
        {
            var envelopeId1 = Guid.NewGuid();
            var envelopeId2 = Guid.NewGuid();

            return new AssignIncomeRequest
            {
                Assignments = new List<AssignIncome>
                {
                    new AssignIncome
                    {
                        Amount = 50,
                        Envelope = new Envelope { Id = envelopeId1 }
                    },
                    new AssignIncome
                    {
                        Amount = 60,
                        Envelope = new Envelope { Id = envelopeId2 }
                    }
                }
            };
        }
    }
}
