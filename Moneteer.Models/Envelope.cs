using Moneteer.Models.Validation;
using System;

namespace Moneteer.Models
{
    public class Envelope : INamedModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EnvelopeCategory EnvelopeCategory { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Balance { get; set; }
        public decimal SpendingLast30Days { get; set; }
    }
}
