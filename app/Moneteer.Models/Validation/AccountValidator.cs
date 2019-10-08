using FluentValidation;

namespace Moneteer.Models.Validation
{
    public class AccountValidator : AbstractValidator<Account>
    {
        public AccountValidator()
        {
            RuleFor(a => a.Name).NotNull().MinimumLength(1).MaximumLength(250);
            RuleFor(a => a.BudgetId).NotEmpty();
        }
    }
}