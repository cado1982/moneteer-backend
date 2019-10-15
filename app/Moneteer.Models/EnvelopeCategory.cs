using Moneteer.Models.Validation;
using System;

namespace Moneteer.Models
{
    public class EnvelopeCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsHidden { get; set; }
    }
}
