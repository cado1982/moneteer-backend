using System;

namespace Moneteer.Models
{
    public class RecentTransactionByEnvelope
    {
        public Guid EnvelopeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Payee { get; set; }
    }
}
