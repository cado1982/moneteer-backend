using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Moneteer.Domain.Repositories
{
    public interface IEnvelopeRepository
    {
        Task<List<Envelope>> GetBudgetEnvelopes(Guid budgetId, IDbConnection conn);
        Task<BudgetEnvelopes> CreateDefaultForBudget(Guid budgetId, IDbConnection conn);
        Task<Envelope> CreateEnvelope(Envelope envelope, IDbConnection conn);
        Task<EnvelopeCategory> CreateEnvelopeCategory(Guid budgetId, EnvelopeCategory envelopeCategory, IDbConnection conn);
        Task AdjustAssigned(Guid envelopeId, decimal assignedAdjustment, IDbConnection conn);
        Task DeleteEnvelope(Guid envelopeId, IDbConnection conn);
        Task<List<EnvelopeCategory>> GetEnvelopeCategories(Guid budgetId, IDbConnection conn);
        Task<Guid> GetEnvelopeOwner(Guid envelopeId, IDbConnection conn);
        Task<Guid> GetEnvelopeCategoryOwner(Guid envelopeCategoryId, IDbConnection conn);
        Task<List<EnvelopeBalance>> GetEnvelopeBalances(Guid budgetId, IDbConnection conn);
        Task MoveEnvelopeBalanceMultiple(Guid fromEnvelopeId, List<Tuple<Guid, decimal>> targets, IDbConnection conn);
        Task UpdateEnvelope(Envelope envelope, IDbConnection conn);
        Task UpdateEnvelopeIsHidden(Guid envelopeId, bool isHidden, IDbConnection conn);
    }
}
