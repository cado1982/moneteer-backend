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
        public decimal Assigned { get; set; }
        public decimal SpendingLast30Days { get; set; }
        public decimal AverageSpend { get; set; }
    }
}
