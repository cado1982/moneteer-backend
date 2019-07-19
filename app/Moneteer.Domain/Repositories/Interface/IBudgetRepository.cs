using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public interface IBudgetRepository
    {
        Task Create(Budget budget, IDbConnection connection);
        Task<Budget> Get(Guid id, IDbConnection connection);
        Task<List<Budget>> GetAllForUser(Guid userId, IDbConnection connection);
        Task<Guid> GetOwner(Guid id, IDbConnection connection);
        Task Delete(Guid id, IDbConnection connection);
        Task AdjustAvailable(Guid id, decimal change, IDbConnection connection);
        Task<decimal> GetAvailable(Guid budgetId, IDbConnection connection);
    }
}
