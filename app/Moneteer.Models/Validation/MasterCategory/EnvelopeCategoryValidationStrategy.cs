using System.Collections.Generic;

namespace Moneteer.Models.Validation
{
    public class EnvelopeCategoryValidationStrategy : DataValidationStrategy<EnvelopeCategory>
    {
        protected override List<ValidationRule<EnvelopeCategory>> Rules => new List<ValidationRule<EnvelopeCategory>>
        {
            new NameValidationRule<EnvelopeCategory>()
        };
    }
}
