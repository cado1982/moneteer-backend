using Moneteer.Models.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moneteer.Models
{
    public class Envelope
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
    }
}
