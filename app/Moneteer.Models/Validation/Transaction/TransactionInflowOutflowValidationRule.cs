using System;

namespace Moneteer.Models.Validation
{
    public class TransactionInflowOutflowValidationRule : ValidationRule<Transaction>
    {
        public override void Validate(Transaction model)
        {
            if (model.Inflow == 0 && model.Outflow == 0)
            {
                throw new ApplicationException("Inflow or Outflow must be provided");
            }
            if (model.Inflow < 0 || model.Outflow < 0)
            {
                throw new ApplicationException("Inflow and Outflow cannot be negative");
            }
            if (model.Inflow > 0 && model.Outflow > 0)
            {
                throw new ApplicationException("Inflow and Outflow cannot both be provided");
            }
        }
    }
}
