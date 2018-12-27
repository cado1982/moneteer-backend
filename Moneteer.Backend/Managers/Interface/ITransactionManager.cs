using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moneteer.Models;

namespace Moneteer.Backend.Managers
{
    public interface ITransactionManager
    {
        Task<List<Transaction>> GetAllForBudget(Guid budgetId, Guid userId);
        Task<List<Transaction>> GetAllForAccount(Guid accountId, Guid userId);
        Task<Transaction> CreateTransaction(Transaction transaction, Guid userId);
        Task DeleteTransactions(List<Guid> transactionIds, Guid userId);
        Task<Transaction> UpdateTransaction(Transaction transaction, Guid userId);
        Task SetTransactionIsCleared(Guid transactionId, bool isCleared, Guid userId);
    }
}
