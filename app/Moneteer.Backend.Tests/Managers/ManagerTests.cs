using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Moneteer.Backend.Tests.Managers
{
    public abstract class ManagerTests
    {
        protected readonly IBudgetRepository BudgetRepository = Mock.Of<IBudgetRepository>();
        protected readonly IAccountRepository AccountRepository = Mock.Of<IAccountRepository>();
        protected readonly IPayeeRepository PayeeRepository = Mock.Of<IPayeeRepository>();
        protected readonly IEnvelopeRepository EnvelopeRepository = Mock.Of<IEnvelopeRepository>();
        protected readonly ITransactionRepository TransactionRepository = Mock.Of<ITransactionRepository>();
        protected readonly ITransactionAssignmentRepository TransactionAssignmentRepository = Mock.Of<ITransactionAssignmentRepository>();
        protected readonly IConnectionProvider ConnectionProvider = Mock.Of<IConnectionProvider>();

        protected readonly IDbConnection DbConnection = Mock.Of<IDbConnection>();
        protected readonly IDbTransaction DbTransaction = Mock.Of<IDbTransaction>();

        protected Guid BudgetId = Guid.NewGuid();
        protected Guid UserId = Guid.NewGuid();
        protected Guid AccountId = Guid.NewGuid();
        protected Guid PayeeId = Guid.NewGuid();
        protected Guid EnvelopeId = Guid.NewGuid();
        protected Guid EnvelopeCategoryId = Guid.NewGuid();

        protected Account Account;
        protected Payee Payee;
        protected Envelope Envelope;
        protected EnvelopeCategory EnvelopeCategory;

        protected Guards Guards;

        public ManagerTests()
        {
            Mock.Get(ConnectionProvider).Setup(p => p.GetConnection()).Returns(DbConnection);
            Mock.Get(ConnectionProvider).Setup(p => p.GetOpenConnection()).Returns(DbConnection);
            Mock.Get(DbConnection).Setup(c => c.BeginTransaction()).Returns(DbTransaction);

            Mock.Get(BudgetRepository).Setup(r => r.GetOwner(BudgetId, DbConnection)).ReturnsAsync(UserId);
            Mock.Get(AccountRepository).Setup(r => r.GetOwner(It.IsAny<Guid>(), DbConnection)).ReturnsAsync(UserId);
            Mock.Get(PayeeRepository).Setup(r => r.GetOwner(It.IsAny<Guid>(), DbConnection)).ReturnsAsync(UserId);
            Mock.Get(TransactionRepository).Setup(r => r.GetOwner(It.IsAny<Guid>(), DbConnection)).ReturnsAsync(UserId);
            Mock.Get(TransactionRepository).Setup(r => r.GetOwners(It.IsAny<List<Guid>>(), DbConnection)).ReturnsAsync(new List<Guid> { UserId });

            Guards = new Guards(new BudgetGuard(BudgetRepository),
                                new AccountGuard(AccountRepository),
                                new PayeeGuard(PayeeRepository),
                                new TransactionGuard(TransactionRepository));

            Account = new Account();
            Account.BudgetId = BudgetId;
            Account.Id = AccountId; 
            Account.Name = "I'm an account";

            Payee = new Payee();
            Payee.Id = PayeeId;
            Payee.Name = "I'm a payee";

            EnvelopeCategory = new EnvelopeCategory
            {
                Id = EnvelopeCategoryId,
                Name = "I'm an envelope category"
            };

            Envelope = new Envelope
            {
                Id = EnvelopeId,
                Name = "I'm an envelope",
                EnvelopeCategory = EnvelopeCategory
            };

            Mock.Get(AccountRepository).Setup(r => r.Get(AccountId, DbConnection))
                                       .ReturnsAsync(Account.ToEntity());

            Mock.Get(PayeeRepository).Setup(r => r.GetPayee(Payee.Id, DbConnection))
                                     .ReturnsAsync(Payee.ToEntity());

            Mock.Get(TransactionRepository).Setup(r => r.CreateTransaction(It.IsAny<Domain.Entities.Transaction>(), DbConnection))
                                           .ReturnsAsync((Domain.Entities.Transaction t, IDbConnection conn) => t);

            Mock.Get(EnvelopeRepository).Setup(r => r.GetBudgetEnvelopes(BudgetId, DbConnection))
                                        .ReturnsAsync(new List<Domain.Entities.Envelope>
                                        {
                                            Envelope.ToEntity()
                                        });
        }

        protected void SetupEnvelope(Action<Envelope> envelopeFunc)
        {
            envelopeFunc(Envelope);

            Mock.Get(EnvelopeRepository).Setup(r => r.GetBudgetEnvelopes(BudgetId, DbConnection))
                                        .ReturnsAsync(new List<Domain.Entities.Envelope>
                                        {
                                            Envelope.ToEntity()
                                        });
        }

        protected void SetupBudgetAvailable(decimal available)
        {
            Mock.Get(BudgetRepository).Setup(r => r.GetAvailable(BudgetId, DbConnection)).ReturnsAsync(available);
        }
    }
}
