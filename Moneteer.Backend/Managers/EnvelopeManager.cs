using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities = Moneteer.Domain.Entities;

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
            await GuardBudget(budgetId, userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var entity = envelope.ToEntity();

                await _envelopeRepository.CreateEnvelope(entity, conn);

                var model = entity.ToModel();

                return model;
            }
        }

        public async Task CreateDefaultEnvelopes(Guid budgetId, Guid userId)
        {
            await GuardBudget(budgetId, userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await _envelopeRepository.CreateDefaultForBudget(budgetId, conn);
            }
        }

        public async Task<EnvelopeCategory> CreateEnvelopeCategory(Guid budgetId, EnvelopeCategory envelopeCategory, Guid userId)
        {
            await GuardBudget(budgetId, userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var entity = envelopeCategory.ToEntity();

                entity = await _envelopeRepository.CreateEnvelopeCategory(budgetId, entity, conn);

                var model = entity.ToModel();

                return model;
            }
        }

        public async Task<List<Envelope>> GetEnvelopes(Guid budgetId, Guid userId)
        {
            if (budgetId == null) throw new ArgumentException("budgetId must be provided");

            await GuardBudget(budgetId, userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var envelopes = await _envelopeRepository.GetBudgetEnvelopes(budgetId, conn);

                var models = envelopes.ToModels();

                return models.ToList();
            }
        }

        public async Task<decimal> GetAvailable(Guid budgetId, Guid userId)
        {
            if (budgetId == null) throw new ArgumentException("budgetId must be provided");

            await GuardBudget(budgetId, userId);

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                var available = await _budgetRepository.GetAvailable(budgetId, conn);

                return available;
            }
        }
    }
}
