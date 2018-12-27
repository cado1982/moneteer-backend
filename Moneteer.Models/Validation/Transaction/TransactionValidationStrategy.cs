using System.Collections.Generic;

namespace Moneteer.Models.Validation
{
    public class TransactionValidationStrategy : DataValidationStrategy<Transaction>
    {
        protected override List<ValidationRule<Transaction>> Rules => new List<ValidationRule<Transaction>>
        {
            new TransactionInflowOutflowValidationRule(),
            new TransactionAccountValidationRule(),
            new TransactionAssignmentsValidationRule(),
            new TransactionFlagsValidationRule(),
            new TransactionPayeeValidationRule()
        };
    }
}
