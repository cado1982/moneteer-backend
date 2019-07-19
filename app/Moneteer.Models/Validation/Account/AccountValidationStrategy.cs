using System.Collections.Generic;

namespace Moneteer.Models.Validation
{
    public class AccountValidationStrategy : DataValidationStrategy<Account>
    {
        protected override List<ValidationRule<Account>> Rules => new List<ValidationRule<Account>>
        {
            new NameValidationRule<Account>()
        };
    }
}
