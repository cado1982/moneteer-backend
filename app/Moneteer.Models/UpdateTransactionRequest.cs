using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Moneteer.Models.Validation;

namespace Moneteer.Models
{
    public class UpdateTransactionRequest : IValidatableObject
    {
        [NotEmpty]
        public Guid TransactionId { get; set; }

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
