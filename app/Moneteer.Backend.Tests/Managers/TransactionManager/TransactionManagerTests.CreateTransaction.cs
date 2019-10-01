using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Moneteer.Models;
using Moneteer.Backend.Extensions;
using System.Collections.Generic;
using Moneteer.Domain.Exceptions;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class TransactionManagerTests : ManagerTests
    {
        [Fact]
        public async Task CreateTransaction_GuardsAccount()
        {
            Mock.Get(AccountRepository).Setup(r => r.GetOwner(AccountId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<ForbiddenException>(() => _sut.CreateTransaction(Transaction, UserId));
        }

        [Fact]
        public async Task CreateTransaction_OpensDbConnection()
        {
            await _sut.CreateTransaction(Transaction, UserId);

            Mock.Get(ConnectionProvider).Verify(s => s.GetOpenConnection(), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_UsesATransaction()
        {
            await _sut.CreateTransaction(Transaction, UserId);

            Mock.Get(DbConnection).Verify(s => s.BeginTransaction(), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_CommitsTheTransaction()
        {
            await _sut.CreateTransaction(Transaction, UserId);

            Mock.Get(DbTransaction).Verify(s => s.Commit(), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_CreatesTransactionAssignments()
        {
            var assignments = new List<TransactionAssignment>
            {
                new TransactionAssignment 
                {
                    Id = Guid.NewGuid(),
                    Inflow = 50,
                    Outflow = 125,
                    Envelope = new Envelope { Id = Guid.NewGuid() }
                },
            };
            Transaction.Assignments = assignments;

            var actual = await _sut.CreateTransaction(Transaction, UserId);

            Mock.Get(TransactionAssignmentRepository).Verify(r => r.CreateTransactionAssignments(It.IsAny<List<Domain.Entities.TransactionAssignment>>(), TransactionId, DbConnection), Times.Once);
        }
    }
}
