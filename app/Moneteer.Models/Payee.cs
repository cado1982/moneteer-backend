using Moneteer.Models.Validation;
using System;

namespace Moneteer.Models
{
    public class Payee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Envelope LastEnvelope { get; set; }
    }
}
