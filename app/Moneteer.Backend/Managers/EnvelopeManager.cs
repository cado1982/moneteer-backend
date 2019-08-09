using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public class EnvelopeManager : BaseManager, IEnvelopeManager
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly IBudgetRepository _budgetRepository;

        public EnvelopeManager(IConnectionProvider connectionProvider,
                               IEnvelopeRepository categoryRepository,
                               IBudgetRepository budgetRepository,
                               Guards guards)
            : base(guards)
        {
            _connectionProvider = connectionProvider;
            _envelopeRepository = categoryRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<Envelope> CreateEnvelope(Guid budgetId, Envelope envelope, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn);
            
                var entity = envelope.ToEntity();

                await _envelopeRepository.CreateEnvelope(entity, conn);

                var model = entity.ToModel();

                return model;
            }
        }

        public async Task CreateDefaultEnvelopes(Guid budgetId, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn);
            
                await _envelopeRepository.CreateDefaultForBudget(budgetId, conn);
            }
        }

        public async Task<EnvelopeCategory> CreateEnvelopeCategory(Guid budgetId, EnvelopeCategory envelopeCategory, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");
            if (envelopeCategory == null || String.IsNullOrWhiteSpace(envelopeCategory.Name)) throw new ArgumentException("Category name must be provided.");

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn);
            
                var entity = envelopeCategory.ToEntity();

                entity = await _envelopeRepository.CreateEnvelopeCategory(budgetId, entity, conn);

                var model = entity.ToModel();

                return model;
            }
        }

        public async Task<List<Envelope>> GetEnvelopes(Guid budgetId, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn);
            
                var envelopes = await _envelopeRepository.GetBudgetEnvelopes(budgetId, conn);

                var models = envelopes.ToModels();

                return models.ToList();
            }
        }

        public async Task<decimal> GetAvailable(Guid budgetId, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn);
            
                return await GetAvailableInternal(budgetId, conn);
            }
        }

        private async Task<decimal> GetAvailableInternal(Guid budgetId, IDbConnection conn)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");

            var available = await _budgetRepository.GetAvailable(budgetId, conn);

            return available;
        }

        public async Task AssignIncome(Guid budgetId, AssignIncomeRequest request, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");

            using (var conn = _connectionProvider.GetOpenConnection())
            using (var transaction = conn.BeginTransaction())
            {
                await GuardBudget(budgetId, userId, conn);
            
                var available = await GetAvailableInternal(budgetId, conn);

                var requestAssignmentTotal = request.Assignments.Sum(a => a.Amount);

                if (requestAssignmentTotal > available)
                {
                    throw new ApplicationException("Attempt to assign more income than is available.");
                }

                foreach (var assignment in request.Assignments)
                {
                    await _envelopeRepository.AdjustBalance(assignment.Envelope.Id, assignment.Amount, conn);
                }

                await _budgetRepository.AdjustAvailable(budgetId, -requestAssignmentTotal, conn);

                transaction.Commit();
            }
        }

        public async Task<List<EnvelopeCategory>> GetEnvelopeCategories(Guid budgetId, Guid userId)
        {
            if (budgetId == Guid.Empty) throw new ArgumentException("budgetId must be provided");

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardBudget(budgetId, userId, conn);

                var envelopeCategories = await _envelopeRepository.GetEnvelopeCategories(budgetId, conn);

                var models = envelopeCategories.ToModels();

                return models.ToList();
            }
        }
    }
}
