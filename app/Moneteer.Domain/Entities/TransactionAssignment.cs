using System;

namespace Moneteer.Domain.Entities
{
    public class TransactionAssignment
    {
        public Guid Id { get; set; }
        public decimal Inflow { get; set; }
        public decimal Outflow { get; set; }
        public Envelope Envelope { get; set; }
        public Account Account { get; set; }
    }
}
