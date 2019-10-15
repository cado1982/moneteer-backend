using System;
using System.ComponentModel.DataAnnotations;

namespace Moneteer.Models
{
    public class Envelope
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public EnvelopeCategory EnvelopeCategory { get; set; }
        public bool IsHidden { get; set; }
        public decimal Balance { get; set; }
        public decimal SpendingLast30Days { get; set; }
        public decimal AverageSpend { get; set; }
    }
}
