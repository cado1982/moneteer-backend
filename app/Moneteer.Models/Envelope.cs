using Moneteer.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moneteer.Models
{
    public class Envelope : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        public EnvelopeCategory EnvelopeCategory { get; set; }

        public bool IsHidden { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Balance { get; set; }
        public decimal SpendingLast30Days { get; set; }
        public decimal AverageSpend { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validation only for newly created envelopes
            if (Id == Guid.Empty) {
                if (EnvelopeCategory == null || EnvelopeCategory.Id == Guid.Empty) {
                    yield return new ValidationResult("Envelopes must have a category");
                }
            }
        }
    }
}
