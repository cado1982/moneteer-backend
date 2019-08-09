using System;

namespace Moneteer.Domain.Entities
{
    public class RecentTransactionByEnvelope
    {
        public Guid EnvelopeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Payee { get; set; }
    }
}
