using System;

namespace Moneteer.Domain.Entities
{
    public class Envelope
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EnvelopeCategory EnvelopeCategory { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Balance { get; set; }
    }
}
