using Moneteer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moneteer.Backend.Managers
{
    public interface IEnvelopeManager
    {
        Task<List<Envelope>> GetEnvelopes(Guid budgetId, Guid userId);
        Task CreateDefaultEnvelopes(Guid budgetId, Guid userId);
        Task<EnvelopeCategory> CreateEnvelopeCategory(Guid budgetId, EnvelopeCategory envelopeCategory, Guid userId);
        Task<Envelope> CreateEnvelope(Guid budgetId, Envelope envelope, Guid userId);
        Task<decimal> GetAvailable(Guid budgetId, Guid userId);
        Task AssignIncome(Guid budgetId, AssignIncomeRequest request, Guid userId);
        Task<List<EnvelopeCategory>> GetEnvelopeCategories(Guid budgetId, Guid userId);
    }
}
