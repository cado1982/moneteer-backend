using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Moneteer.Domain.Guards
{
    public class BudgetGuard
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IConnectionProvider _connectionProvider;

        public BudgetGuard(IBudgetRepository budgetRepository, IConnectionProvider connectionProvider)
        {
            _budgetRepository = budgetRepository;
            _connectionProvider = connectionProvider;
        }

        public async Task Guard(Guid budgetId, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided", nameof(budgetId));

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var budgetOwnerId = await _budgetRepository.GetOwner(budgetId, conn);

                if (budgetOwnerId != userId)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
    }
}
