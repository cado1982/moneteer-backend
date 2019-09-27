using System;

namespace Moneteer.Models.Validation
{
    public class BudgetValidationRule : ValidationRule<Budget>
    {
        public override void Validate(Budget model)
        {
            if (String.IsNullOrWhiteSpace(model.CurrencyFormat.ThousandsSeparator))
            {
                throw new ApplicationException("Thousands Separator must be provided");
            }
            if (String.IsNullOrWhiteSpace(model.CurrencyFormat.DecimalSeparator))
            {
                throw new ApplicationException("Thousands Separator must be provided");
            }
            if (model.CurrencyFormat.DecimalPlaces < 0)
            {
                throw new ApplicationException("Decimal Places cannot be less than 0");
            }
            if (String.IsNullOrWhiteSpace(model.Currency.Code))
            {
                throw new ApplicationException("Currency Code must be provided");
            }
            if (String.IsNullOrWhiteSpace(model.DateFormat))
            {
                throw new ApplicationException("Date Format must be provided");
            }
        }
    }
}
