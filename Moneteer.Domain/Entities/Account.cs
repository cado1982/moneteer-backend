using System;

namespace Moneteer.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid BudgetId { get; set; }
        public string Name { get; set; }
        public bool IsBudget { get; set; }
    }
}
