using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public interface IBudgetManager
    {
        Task<List<Budget>> GetAllForUser(Guid userId);
        Task<Budget> Get(Guid budgetId, Guid userId);
        Task<Budget> Create(Budget budget, Guid userId);
        Task Delete(Guid budgetId, Guid userId);
    }
}