using Moneteer.Backend.Managers;
using Moneteer.Models.Validation;
using Moq;
using Moneteer.Models;
using System;
using System.Collections.Generic;
using Moneteer.Backend.Extensions;
using System.Linq;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class TransactionManagerTests : ManagerTests
    {
        private readonly TransactionManager _sut;

        protected readonly IDataValidationStrategy<Transaction> ValidationStrategy = Mock.Of<IDataValidationStrategy<Transaction>>();
        
        protected Guid TransactionId = Guid.NewGuid();
        protected Guid TransactionAssignmentId = Guid.NewGuid();
        protected Transaction Transaction;

        public TransactionManagerTests()
        {
            _sut = new TransactionManager(
                TransactionRepository,
                AccountRepository,
                TransactionAssignmentRepository,
                PayeeRepository,
                EnvelopeRepository,
                BudgetRepository,
                ValidationStrategy,
                Guards,
                ConnectionProvider
            );

            Transaction = new Transaction
            {
                Account = Account,
                Id = TransactionId,
                Assignments = new List<TransactionAssignment>()
            };
        
            Mock.Get(TransactionRepository).Setup(r => r.GetById(TransactionId, DbConnection)).ReturnsAsync(Transaction.ToEntity());
        }

        private void SetupTransaction(Action<Transaction> transactionAction)
        {
            transactionAction(Transaction);

            Mock.Get(TransactionRepository).Setup(r => r.GetAllForBudget(BudgetId, DbConnection)).ReturnsAsync(new List<Domain.Entities.Transaction> { Transaction.ToEntity() });
            Mock.Get(TransactionRepository).Setup(r => r.GetById(TransactionId, DbConnection)).ReturnsAsync(Transaction.ToEntity());
        }
    }
}
