using System;

namespace Moneteer.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsBudget { get; set; }
        public decimal InitialBalance { get; set; }
        public Guid BudgetId { get; set; }
        public decimal ClearedBalance { get; set; }
        public decimal UnclearedBalance { get; set; }
    }
}
