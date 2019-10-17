using Moneteer.Backend.Extensions;
using Moneteer.Domain.Guards;
using Moneteer.Domain.Helpers;
using Moneteer.Domain.Repositories;
using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public class EnvelopeManager : BaseManager, IEnvelopeManager
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly IEnvelopeRepository _envelopeRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly ITransactionRepository _transactionRepository;

        public EnvelopeManager(IConnectionProvider connectionProvider,
                               IEnvelopeRepository categoryRepository,
                               IBudgetRepository budgetRepository,
                               ITransactionRepository transactionRepository,
                               Guards guards)
            : base(guards)
        {
            _connectionProvider = connectionProvider;
            _envelopeRepository = categoryRepository;
            _budgetRepository = budgetRepository;
            _transactionRepository = transactionRepository;
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
                var envelopeBalances = await _envelopeRepository.GetEnvelopeBalances(budgetId, conn);

                var models = envelopes.ToModels().ToList();

                foreach (var model in models)
                {
                    var balance = envelopeBalances.SingleOrDefault(b => b.EnvelopeId == model.Id);
                    model.Balance = balance == null ? 0 : balance.Balance;
                }

                return models;
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

        public async Task DeleteEnvelope(Guid envelopeId, Guid userId)
        {
            if (envelopeId == Guid.Empty) throw new ArgumentException("envelope id must be provided");

            using (var conn = _connectionProvider.GetOpenConnection())
            using (var trans = conn.BeginTransaction())
            {
                await GuardEnvelope(envelopeId, userId, conn);

                var envelope = await _envelopeRepository.GetEnvelope(envelopeId, conn);

                if (envelope == null) throw new ApplicationException("Envelope does not exist");
                if (envelope.Assigned < 0) throw new ApplicationException("Cannot delete envelopes who have a negative balance");

                // Throw if any transactions are still using this envelope
                var transactions = await _transactionRepository.GetByEnvelopeId(envelopeId, conn);

                if (transactions.Any()) {
                    throw new ApplicationException("Cannot delete an envelope that is being used in a transaction. Change these transactions to a different envelope first.");
                }

                // Move any existing balance to the Available Income envelope
                if (envelope.Assigned > 0) 
                {
                    var availableIncomeEnvelopeId = await _envelopeRepository.GetAvailableIncomeEnvelopeId(envelope.EnvelopeCategory.BudgetId, conn);
                    var target = new Tuple<Guid, decimal>(availableIncomeEnvelopeId, envelope.Assigned);

                    await _envelopeRepository.MoveEnvelopeBalanceMultiple(envelopeId, new List<Tuple<Guid, decimal>>{ target }, conn);
                }

                await _envelopeRepository.DeleteEnvelope(envelopeId, conn);

                trans.Commit();
            }
        }

        public async Task MoveEnvelopeBalance(Guid fromEnvelopeId, List<EnvelopeBalanceTarget> targets, Guid userId)
        {
            if (fromEnvelopeId == Guid.Empty) throw new ArgumentException("fromEnvelopeId must be provided");
            if (targets == null || !targets.Any()) throw new ArgumentException("requests must be provided");
            if (targets.Any(r => r.Amount <= 0)) throw new ArgumentException("All requests must have positive amount");
            if (targets.Any(r => r.EnvelopeId == fromEnvelopeId)) throw new ArgumentException("None of the requests can target the same envelope as fromEnvelopeId");

            using (var conn = _connectionProvider.GetOpenConnection())
            using (var trans = conn.BeginTransaction())
            {
                await GuardEnvelope(fromEnvelopeId, userId, conn);

                foreach (var targetEnvelopeId in targets.Select(t => t.EnvelopeId).Distinct())
                {
                    await GuardEnvelope(targetEnvelopeId, userId, conn);
                }

                var tupleRequests = targets.Select(r => new Tuple<Guid, decimal>(r.EnvelopeId, r.Amount)).ToList();
                await _envelopeRepository.MoveEnvelopeBalanceMultiple(fromEnvelopeId, tupleRequests, conn);

                trans.Commit();
            }
        }

        public async Task UpdateEnvelope(Envelope envelope, Guid userId)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));
            if (envelope.Id == Guid.Empty) throw new ArgumentException("Envelope id must be provided", nameof(envelope));
            if (envelope.EnvelopeCategory == null) throw new ArgumentException("Envelope category must be provided", nameof(envelope));
            if (envelope.EnvelopeCategory.Id == Guid.Empty) throw new ArgumentException("Envelope category id must be provided", nameof(envelope));

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardEnvelope(envelope.Id, userId, conn);

                var entity = envelope.ToEntity();

                await _envelopeRepository.UpdateEnvelope(entity, conn);
            }
        }

        public async Task HideEnvelope(Guid envelopeId, Guid userId)
        {
            if (envelopeId == Guid.Empty) throw new ArgumentException("envelopeid must be provided", nameof(envelopeId));
            if (userId == Guid.Empty) throw new ArgumentException("envelopeid must be provided", nameof(userId));

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardEnvelope(envelopeId, userId, conn);

                await _envelopeRepository.UpdateEnvelopeIsHidden(envelopeId, true, conn);
            }
        }

        public async Task ShowEnvelope(Guid envelopeId, Guid userId)
        {
            if (envelopeId == Guid.Empty) throw new ArgumentException("envelopeid must be provided", nameof(envelopeId));
            if (userId == Guid.Empty) throw new ArgumentException("envelopeid must be provided", nameof(userId));

            using (var conn = _connectionProvider.GetOpenConnection())
            {
                await GuardEnvelope(envelopeId, userId, conn);

                await _envelopeRepository.UpdateEnvelopeIsHidden(envelopeId, false, conn);
            }
        }
    }
}