using System;

namespace Moneteer.Models
{
    public class MoveEnvelopeBalanceRequest
    {
        public Guid ToEnvelopeId { get; set; }
        public decimal Amount { get; set; }
    }
}
