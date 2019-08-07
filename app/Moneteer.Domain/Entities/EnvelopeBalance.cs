using System;

namespace Moneteer.Domain.Entities
{
    public class EnvelopeBalance
    {
        public Guid EnvelopeId { get; set; }
        public decimal Balance { get; set; }
    }
}
