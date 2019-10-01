using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moneteer.Models.Validation;

namespace Moneteer.Models
{
    public class Transaction : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required]
        public Account Account { get; set; }

        public bool IsCleared { get; set; }

        public bool IsReconciled { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [MinLength(1)]
        public List<TransactionAssignment> Assignments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsReconciled && !IsCleared) {
                yield return new ValidationResult("A transaction that is reconciled must also be cleared");
            }
            if (Assignments.GroupBy(a => a.Envelope.Id).Any(g => g.Count() > 1))
            {
                yield return new ValidationResult("No duplicate envelopes allowed");
            }
        }
    }

    public class CreateTransactionRequest : IValidatableObject
    {
        [NotEmpty]
        public Guid AccountId { get; set; }

        public bool IsCleared { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [MinLength(1)]
        public List<TransactionAssignment> Assignments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Assignments.GroupBy(a => a.Envelope.Id).Any(g => g.Count() > 1))
            {
                yield return new ValidationResult("No duplicate envelope assignments allowed");
            }
        }
    }
}
