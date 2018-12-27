using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task Create(Account account, IDbConnection connection);
        Task<Guid> GetOwner(Guid accountId, IDbConnection connection);
        Task<Account> Get(Guid accountId, IDbConnection connection);
        Task Delete(Guid accountId, IDbConnection connection);
        Task<List<Account>> GetAllForBudget(Guid budgetId, IDbConnection connection);
        Task Update(Account account, IDbConnection connection);
    }
}
