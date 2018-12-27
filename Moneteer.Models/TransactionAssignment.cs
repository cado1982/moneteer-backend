using System;

namespace Moneteer.Models
{
    public class TransactionAssignment
    {
        public Guid Id { get; set; }
        public decimal Inflow { get; set; }
        public decimal Outflow { get; set; }
        public Envelope Envelope { get; set; }
    }
}
