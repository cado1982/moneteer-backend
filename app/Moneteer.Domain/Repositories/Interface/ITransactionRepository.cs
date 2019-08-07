using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllForAccount(Guid accountId, IDbConnection connection);
        Task<List<Transaction>> GetAllForBudget(Guid budgetId, IDbConnection connection);
        Task<List<Transaction>> GetForMonth(Guid budgetId, short year, short month, IDbConnection connection);
        Task<List<Transaction>> GetOnOrBefore(Guid budgetId, short year, short month, IDbConnection connection);
        Task<List<Transaction>> GetByIds(List<Guid> transactionIds, IDbConnection connection);
        Task<Transaction> GetById(Guid transactionId, IDbConnection connection);

        /// <summary>
        /// Gets transactions that come from budget accounts. A budget account has the isBudget flag set to true
        /// </summary>
        Task<List<Transaction>> GetBudgetOnOrBefore(Guid budgetId, short year, short month, IDbConnection connection);
        Task<Transaction> CreateTransaction(Transaction transaction, IDbConnection connection);
        Task DeleteTransactions(List<Guid> transactionIds, IDbConnection connection);
        Task UpdateTransaction(Transaction transaction, IDbConnection connection);
        Task<Guid> GetOwner(Guid transactionId, IDbConnection connection);
        Task<List<Guid>> GetOwners(List<Guid> transactionIds, IDbConnection connection);
        Task SetIsCleared(Guid transactionId, bool isCleared, IDbConnection connection);
    }
}
