﻿using Moneteer.Models;
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
        Task<Envelope> CreateEnvelope(Envelope envelope, Guid userId);
        Task<List<EnvelopeCategory>> GetEnvelopeCategories(Guid budgetId, Guid userId);
        Task DeleteEnvelope(Guid envelopeId, Guid userId);
        Task UpdateEnvelope(Envelope envelope, Guid userId);
        Task MoveEnvelopeBalance(Guid fromEnvelopeId, List<EnvelopeBalanceTarget> targets, Guid userId);
        Task HideEnvelope(Guid envelopeId, Guid userId);
        Task ShowEnvelope(Guid envelopeId, Guid userId);
        Task HideEnvelopeCategory(Guid envelopeCategoryId, Guid userId);
        Task ShowEnvelopeCategory(Guid envelopeCategoryId, Guid userId);
    }
}
