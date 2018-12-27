using System.Collections.Generic;

namespace Moneteer.Models.Validation
{
    public class EnvelopeValidationStrategy : DataValidationStrategy<Envelope>
    {
        protected override List<ValidationRule<Envelope>> Rules => new List<ValidationRule<Envelope>>
        {
            new NameValidationRule<Envelope>(),
            new EnvelopeCategoryValidationRule()
        };
    }
}
