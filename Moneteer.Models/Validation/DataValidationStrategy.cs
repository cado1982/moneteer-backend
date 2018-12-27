using System.Collections.Generic;

namespace Moneteer.Models.Validation
{
    public abstract class DataValidationStrategy<T> where T : class
    {
        protected abstract List<ValidationRule<T>> Rules { get; }

        public void RunRules(T value)
        {
            foreach (var rule in Rules)
            {
                rule.Validate(value);
            }
        }
    }
}
