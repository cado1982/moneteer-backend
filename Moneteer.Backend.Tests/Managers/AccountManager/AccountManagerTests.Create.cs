using Moneteer.Backend.Managers;
using Moneteer.Models.Validation;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class AccountManagerTests : ManagerTests
    {
        [Fact]
        public void Create_GuardsBudget()
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            var account = new Models.Account()
            {
                BudgetId = BudgetId
            };

            Func<Task> testCase = () => _sut.Create(account, UserId);

            testCase.Should().Throw<UnauthorizedAccessException>();
        }

        [Fact]
        public void Create_FailsEmptyName()
        {
            var account = new Models.Account
            {
                Name = "", 
                BudgetId = BudgetId 
            };

            Func<Task> testCase = () => _sut.Create(account, UserId);

            testCase.Should().Throw<ApplicationException>().WithMessage("Name must be provided");
        }

        [Fact]
        public async Task Create_CallsAccountRepository()
        {
            var account = new Models.Account
            {
                Name = "MyAccount", 
                BudgetId = BudgetId ,
                IsBudget = true,
                InitialBalance = 0
            };

            Mock.Get(AccountRepository)
                .Setup(r => r.Create(It.Is<Domain.Entities.Account>(a => AccountComparison(a, account)
            ), DbConnection)).Returns(Task.CompletedTask).Verifiable();

            await _sut.Create(account, UserId);

            Mock.Get(AccountRepository).Verify();
        }

        [InlineData(123, 123, 0)]
        [InlineData(-123, 0, 123)]
        [Theory]
        public async Task Create_CreatesInitialBalance_WhenPositive(decimal initialBalance, decimal expectedInflow, decimal expectedOutflow)
        {
            var account = new Models.Account
            {
                Name = "MyAccount", 
                BudgetId = BudgetId ,
                IsBudget = true,
                InitialBalance = initialBalance
            };

            var expectedTransactionId = Guid.NewGuid();
            var expectedTransaction = new Domain.Entities.Transaction
            {
                Id = expectedTransactionId
            };

            Mock.Get(TransactionRepository)
                .Setup(r => r.CreateTransaction(It.Is<Domain.Entities.Transaction>(t =>
                    AccountComparison(t.Account, account) &&
                    t.IsCleared &&
                    t.Date == DateTime.UtcNow.Date &&
                    t.Inflow == expectedInflow && 
                    t.Outflow == expectedOutflow &&
                    t.Description == "Automatically entered by Moneteer"
            ), DbConnection)).Returns(Task.FromResult(expectedTransaction)).Verifiable();

            Mock.Get(TransactionAssignmentRepository)
                .Setup(r => r.CreateTransactionAssignments(It.Is<List<Domain.Entities.TransactionAssignment>>(a => 
                    a.Single().Inflow == expectedInflow &&
                    a.Single().Outflow == expectedOutflow
            ), expectedTransactionId, DbConnection)).Returns(Task.CompletedTask).Verifiable();

            await _sut.Create(account, UserId);

            Mock.Get(TransactionRepository).Verify();
            Mock.Get(TransactionAssignmentRepository).Verify();
        }
        
        private bool AccountComparison(Domain.Entities.Account entity, Models.Account model)
        {
            return entity.Name == model.Name &&
            entity.BudgetId == model.BudgetId &&
            entity.IsBudget == model.IsBudget;
        }
    }
}
