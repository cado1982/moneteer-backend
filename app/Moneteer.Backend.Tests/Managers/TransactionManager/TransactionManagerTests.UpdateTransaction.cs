using Moneteer.Backend.Extensions;
using Moneteer.Domain.Exceptions;
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

            await Assert.ThrowsAsync<ForbiddenException>(() => _sut.UpdateTransaction(Transaction, UserId));
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
