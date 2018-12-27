using System;

namespace Moneteer.Domain.Entities
{
    public class EnvelopeCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BudgetId { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDeleted { get; set; }
    }
}
