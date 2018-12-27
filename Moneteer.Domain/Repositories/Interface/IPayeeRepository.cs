using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public interface IPayeeRepository
    {
        Task<List<Payee>> GetAllForBudget(Guid budgetId, IDbConnection connection);
        Task<Payee> CreatePayee(Payee payee, IDbConnection connection);
        Task UpdatePayee(Payee payee, IDbConnection connection);
        Task DeletePayee(Guid payeeId, IDbConnection connection);
        Task<Payee> GetPayee(Guid budgetId, string name, IDbConnection connection);
        Task<Guid> GetOwner(Guid payeeId, IDbConnection connection);
    }
}
