using Moneteer.Models.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moneteer.Models
{
    public class Account
    {
        [NotEmpty]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }
        
        public bool IsBudget { get; set; }
        public decimal InitialBalance { get; set; }

        [NotEmpty]
        public Guid BudgetId { get; set; }
        public decimal ClearedBalance { get; set; }
        public decimal UnclearedBalance { get; set; }
    }

    public class CreateAccountRequest
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [NotEmpty]
        public Guid BudgetId { get; set; }

        public decimal InitialBalance { get; set; }
    }
}
