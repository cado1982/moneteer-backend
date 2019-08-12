﻿using Moneteer.Backend.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class TransactionManagerTests : ManagerTests
    {
        [Fact]
        public async Task UpdateTransaction_GuardsTransaction()
        {
            Mock.Get(TransactionRepository).Setup(r => r.GetOwners(new List<Guid> { TransactionId }, DbConnection))
                                           .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _sut.UpdateTransaction(Transaction, UserId));
        }

        [Fact]
        public async Task UpdateTransaction_RunsValidation()
        {
            await _sut.UpdateTransaction(Transaction, UserId);

            Mock.Get(ValidationStrategy).Verify(s => s.RunRules(Transaction), Times.Once);
        }

        [Fact]
        public async Task UpdateTransaction_WhenTransactionNotFound_ThrowsException()
        {
            Mock.Get(TransactionRepository).Setup(r => r.GetById(TransactionId, DbConnection))
                                           .ReturnsAsync((Domain.Entities.Transaction)null);

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _sut.UpdateTransaction(Transaction, UserId));
            Assert.Equal("Transaction not found", exception.Message);
        }

        [Fact]
        public async Task UpdateTransaction_AdjustsBudgetAvailable()
        {
            SetupBudgetAvailable(10);
            SetupTransaction(t => t.Inflow = 50);

            var updatedTransaction = new Models.Transaction
            {
                Id = Transaction.Id,
                Inflow = 62,
                Assignments = new List<Models.TransactionAssignment>()
            };

            await _sut.UpdateTransaction(updatedTransaction, UserId);

            Mock.Get(BudgetRepository).Verify(r => r.AdjustAvailable(BudgetId, 12, DbConnection), Times.Once);
        }
        
        [Fact]
        public async Task UpdateTransaction_DeletesAssignments()
        {
            await _sut.UpdateTransaction(Transaction, UserId);

            Mock.Get(TransactionAssignmentRepository).Verify(t => t.DeleteTransactionAssignmentsByTransactionId(TransactionId, DbConnection), Times.Once);
        }

        [Fact]
        public async Task UpdateTransaction_RecreatesAssignments()
        {
            await _sut.UpdateTransaction(Transaction, UserId);

            Mock.Get(TransactionAssignmentRepository).Verify(t => t.CreateTransactionAssignments(It.Is<List<Domain.Entities.TransactionAssignment>>(a => a.Count == Transaction.Assignments.Count), TransactionId, DbConnection), Times.Once);
        }

        [Fact]
        public async Task UpdateTransaction_CommitsDbTransaction()
        {
            await _sut.UpdateTransaction(Transaction, UserId);

            Mock.Get(DbTransaction).Verify(t => t.Commit());
        }

        [Fact]
        public async Task UpdateTransaction_ReturnsUpdatedTransaction()
        {
            var transaction = await _sut.UpdateTransaction(Transaction, UserId);

            Assert.NotNull(transaction);
            Assert.Equal(Transaction.Id, transaction.Id);
        }
    }
}
