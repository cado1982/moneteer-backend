using System;

namespace Moneteer.Models.Validation
{
    public class TransactionAccountValidationRule : ValidationRule<Transaction>
    {
        public override void Validate(Transaction model)
        {
            if (model.Account == null || model.Account.Id == Guid.Empty)
            {
                throw new ApplicationException("A transaction must be linked to an account");
            }
        }
    }
}
