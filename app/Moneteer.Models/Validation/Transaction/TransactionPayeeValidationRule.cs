using System;

namespace Moneteer.Models.Validation
{
    public class TransactionPayeeValidationRule : ValidationRule<Transaction>
    {
        public override void Validate(Transaction model)
        {
            if (model.Payee != null && model.Payee.Id == Guid.Empty && String.IsNullOrWhiteSpace(model.Payee.Name))
            {
                throw new ApplicationException("Payee name must be provided");
            }
        }
    }
}
