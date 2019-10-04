using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moneteer.Models
{
    public class TransactionAssignment : IValidatableObject
    {
        public Guid Id { get; set; }

        [Range(0, Double.MaxValue)]
        public decimal Inflow { get; set; }
        
        [Range(0, Double.MaxValue)]
        public decimal Outflow { get; set; }

        public Envelope Envelope { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Envelope == null || Envelope.Id == Guid.Empty) {
                yield return new ValidationResult("Envelope must be provided", new [] { nameof(Envelope) });
            }
            if (Inflow > 0 && Outflow > 0) {
                yield return new ValidationResult("Cannot have outflow and inflow set at the same time");
            }
            if (Inflow == 0 && Outflow == 0) {
                yield return new ValidationResult("Must set either inflow or outflow");
            }
        }
    }
}
