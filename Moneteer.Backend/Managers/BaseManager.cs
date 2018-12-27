using Moneteer.Domain.Guards;
using System;
using System.Collections.Generic;
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

        protected Task GuardBudget(Guid budgetId, Guid userId)
        {
            return _guards.BudgetGuard.Guard(budgetId, userId);
        }

        protected Task GuardAccount(Guid accountId, Guid userId)
        {
            return _guards.AccountGuard.Guard(accountId, userId);
        }

        protected Task GuardPayee(Guid payeeId, Guid userId)
        {
            return _guards.PayeeGuard.Guard(payeeId, userId);
        }

        protected Task GuardTransaction(Guid transactionId, Guid userId)
        {
            return _guards.TransactionGuard.Guard(transactionId, userId);
        }

        protected Task GuardTransactions(List<Guid> transactionIds, Guid userId)
        {
            return _guards.TransactionGuard.Guard(transactionIds, userId);
        }
    }
}
