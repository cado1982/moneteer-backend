using Moneteer.Backend.Managers;
using Moneteer.Models.Validation;
using Moq;
using Moneteer.Models;
using System;
using System.Collections.Generic;

namespace Moneteer.Backend.Tests.Managers
{
    public partial class TransactionManagerTests : ManagerTests
    {
        private readonly TransactionManager _sut;

        protected readonly IDataValidationStrategy<Transaction> ValidationStrategy = Mock.Of<IDataValidationStrategy<Transaction>>();

        protected Transaction Transaction;

        protected Guid TransactionId = Guid.NewGuid();

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

            Transaction = new Transaction();
            Transaction.Account = Account;
            Transaction.Id = TransactionId;
            Transaction.Assignments = new List<TransactionAssignment>();
        }
    }
}
