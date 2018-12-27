using System;

namespace Moneteer.Models.Validation
{
    public class EnvelopeCategoryValidationRule : ValidationRule<Models.Envelope>
    {
        public override void Validate(Models.Envelope model)
        {
            if (model.EnvelopeCategory == null || model.EnvelopeCategory.Id == Guid.Empty)
            {
                throw new ApplicationException("Envelope category must be provided");
            }
        }
    }
}
