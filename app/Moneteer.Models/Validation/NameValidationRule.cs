using System;

namespace Moneteer.Models.Validation
{
    public class NameValidationRule<T> : ValidationRule<T> where T : class, INamedModel
    {
        public override void Validate(T model)
        {
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ApplicationException("Name must be provided");
            }
        }
    }
}
