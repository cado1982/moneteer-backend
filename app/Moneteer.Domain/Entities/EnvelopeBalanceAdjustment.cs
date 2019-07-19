namespace Moneteer.Domain.Entities
{
    public class EnvelopeBalanceAdjustment
    {
        public decimal Adjustment { get; set; }
        public Envelope Envelope { get; set; }
    }
}
