using System;

namespace Moneteer.Models.Validation
{
    public class TransactionFlagsValidationRule : ValidationRule<Transaction>
    {
        public override void Validate(Transaction model)
        {
            if (model.IsReconciled && !model.IsCleared)
            {
                throw new ApplicationException("Reconciled transactions must also be cleared");
            }
        }
    }
}
