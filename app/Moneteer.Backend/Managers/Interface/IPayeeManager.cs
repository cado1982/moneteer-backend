using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public interface IPayeeManager
    {
        Task<List<Payee>> GetAllForBudget(Guid budgetId, Guid userId);
        Task DeletePayee(Guid payeeId, Guid userId);
        Task UpdatePayee(Payee payee, Guid userId);
    }
}
