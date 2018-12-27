using Moneteer.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Moneteer.Domain.Entities
{
    public class BudgetEnvelopes
    {
        public List<EnvelopeCategory> Categories { get; set; }
        public List<Envelope> Envelopes { get; set; }
    }
}
