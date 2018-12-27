using System;

namespace Moneteer.Domain.Entities
{
    public class Payee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BudgetId { get; set; }
        public Envelope LastEnvelope { get; set; }
    }
}
