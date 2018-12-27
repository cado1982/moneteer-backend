using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models.Validation;
using Entities = Moneteer.Domain.Entities;

namespace Moneteer.Backend.Managers
{
    public class BudgetManager : BaseManager, IBudgetManager
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IEnvelopeRepository _categoryRepository;
        private readonly BudgetValidationStrategy _validationStrategy;
        private readonly IConnectionProvider _connectionProvider;

        public BudgetManager(IBudgetRepository budgetRepository,
                             IEnvelopeRepository categoryRepository,
                             BudgetValidationStrategy validationStrategy,
                             IConnectionProvider connectionProvider,
                             Guards guards)
            : base(guards)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
            _validationStrategy = validationStrategy;
            _connectionProvider = connectionProvider;
        }

        public async Task<Models.Budget> Create(Models.Budget budget, Guid userId)
        {
            _validationStrategy.RunRules(budget);

            var entity = budget.ToEntity(userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            using (var transaction = conn.BeginTransaction())
            {
                {
                    await _budgetRepository.Create(entity, conn).ConfigureAwait(false);
                    await _categoryRepository.CreateDefaultForBudget(entity.Id, conn).ConfigureAwait(false);

                    transaction.Commit();
                }
            }

            var model = new Models.Budget
            {
                Id = entity.Id,
                Name = entity.Name
            };

            return model;
        }

        public async Task Delete(Guid budgetId, Guid userId)
        {
            await GuardBudget(budgetId, userId).ConfigureAwait(false);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await _budgetRepository.Delete(budgetId, conn).ConfigureAwait(false);
            }
        }

        public async Task<Models.Budget> Get(Guid budgetId, Guid userId)
        {
            await GuardBudget(budgetId, userId).ConfigureAwait(false);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var entity = await _budgetRepository.Get(budgetId, conn).ConfigureAwait(false);

                var model = entity.ToModel();

                return model;
            }
        }

        public async Task<List<Models.Budget>> GetAllForUser(Guid userId)
        {
            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var entities = await _budgetRepository.GetAllForUser(userId, conn).ConfigureAwait(false);

                var models = entities.Select(e => e.ToModel()).ToList();

                return models;
            }
        }

        public async Task AdjustAvailable(Guid budgetId, Guid userId, decimal change)
        {
            await GuardBudget(budgetId, userId).ConfigureAwait(false);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await _budgetRepository.AdjustAvailable(budgetId, change, conn);
            }
        }
    }
}
