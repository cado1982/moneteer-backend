using Moneteer.Domain.Exceptions;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class BudgetGuard
    {
        private readonly IBudgetRepository _budgetRepository;

        public BudgetGuard(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task Guard(Guid budgetId, Guid userId, IDbConnection conn)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));
            if (userId == Guid.Empty) throw new ArgumentException("userId must be provided", nameof(userId));

            var budgetOwnerId = await _budgetRepository.GetOwner(budgetId, conn);

            if (budgetOwnerId != userId)
            {
                throw new ForbiddenException();
            }
        }
    }
}
