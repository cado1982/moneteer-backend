using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Moneteer.Models;
using Moneteer.Backend.Extensions;
using System.Collections.Generic;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class TransactionManagerTests : ManagerTests
    {
        [Fact]
        public async Task CreateTransaction_GuardsAccount()
        {
            Mock.Get(AccountRepository).Setup(r => r.GetOwner(AccountId, DbConnection)).ReturnsAsync(Guid.NewGuid());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.CreateTransaction(Transaction, UserId));
        }

        [Fact]
        public async Task CreateTransaction_RunsValidationRules()
        {
            await _sut.CreateTransaction(Transaction, UserId);

            Mock.Get(ValidationStrategy).Verify(s => s.RunRules(Transaction), Times.Once);
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
        public async Task CreateTransaction_WhenNewPayeeProvided_CreateNewPayee()
        {
            Transaction.Payee = new Payee();

            var actual = await _sut.CreateTransaction(Transaction, UserId);

            Mock.Get(PayeeRepository).Verify(r => r.CreatePayee(It.Is<Domain.Entities.Payee>(p => p.Id == Guid.Empty && p.BudgetId == BudgetId), DbConnection), Times.Once);
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

        [Fact]
        public async Task CreateTransaction_AdjustsEnvelopeBalances()
        {
            var envelope1Guid = Guid.NewGuid();
            var envelope2Guid = Guid.NewGuid();

            var assignments = new List<TransactionAssignment>
            {
                new TransactionAssignment 
                {
                    Id = Guid.NewGuid(),
                    Inflow = 50,
                    Outflow = 125,
                    Envelope = new Envelope { Id = envelope1Guid }
                },
                new TransactionAssignment 
                {
                    Id = Guid.NewGuid(),
                    Inflow = 60,
                    Outflow = 25,
                    Envelope = new Envelope { Id = envelope2Guid }
                },
            };
            Transaction.Assignments = assignments;

            var actual = await _sut.CreateTransaction(Transaction, UserId);

            Mock.Get(EnvelopeRepository).Verify(r => r.AdjustBalance(envelope1Guid, -75, DbConnection), Times.Once);
            Mock.Get(EnvelopeRepository).Verify(r => r.AdjustBalance(envelope2Guid, 35, DbConnection), Times.Once);
        }
    }
}
