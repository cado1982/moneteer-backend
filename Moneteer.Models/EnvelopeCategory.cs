using Moneteer.Models.Validation;
using System;

namespace Moneteer.Models
{
    public class EnvelopeCategory : INamedModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDeleted { get; set; }
    }
}
