using System.Collections.Generic;

namespace Moneteer.Models.Validation
{
    public class BudgetValidationStrategy : DataValidationStrategy<Budget>
    {
        protected override List<ValidationRule<Budget>> Rules => new List<ValidationRule<Budget>>
        {
            new NameValidationRule<Budget>(),
            new BudgetValidationRule()
        };
    }
}
