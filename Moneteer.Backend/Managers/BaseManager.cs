using Moneteer.Domain.Guards;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public abstract class BaseManager
    {
        private readonly Guards _guards;

        public BaseManager(Guards guards)
        {
            _guards = guards;
        }

        protected Task GuardBudget(Guid budgetId, Guid userId, IDbConnection conn)
        {
            return _guards.BudgetGuard.Guard(budgetId, userId, conn);
        }

        protected Task GuardAccount(Guid accountId, Guid userId, IDbConnection conn)
        {
            return _guards.AccountGuard.Guard(accountId, userId, conn);
        }

        protected Task GuardPayee(Guid payeeId, Guid userId, IDbConnection conn)
        {
            return _guards.PayeeGuard.Guard(payeeId, userId, conn);
        }

        protected Task GuardTransaction(Guid transactionId, Guid userId, IDbConnection conn)
        {
            return _guards.TransactionGuard.Guard(transactionId, userId, conn);
        }

        protected Task GuardTransactions(List<Guid> transactionIds, Guid userId, IDbConnection conn)
        {
            return _guards.TransactionGuard.Guard(transactionIds, userId, conn);
        }
    }
}
