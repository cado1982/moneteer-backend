using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public interface IAccountManager
    {
        Task<List<Account>> GetAllForBudget(Guid budgetId, Guid userId);
        Task<Account> Get(Guid accountId, Guid userId);
        Task<Account> Create(Account account, Guid userId);
        Task Delete(Guid accountId, Guid userId);
        Task Update(Account account, Guid userId);
    }
}
