using FluentValidation;

namespace Moneteer.Models.Validation
{
    public class TransactionAssignmentValidator : AbstractValidator<TransactionAssignment>
    {
        public TransactionAssignmentValidator()
        {
            // Inflow and outflow not negative
            RuleFor(ta => ta.Inflow).GreaterThanOrEqualTo(0);
            RuleFor(ta => ta.Outflow).GreaterThanOrEqualTo(0);

            // When inflow greater than 0, not outflow and visa versa
            RuleFor(ta => ta.Inflow).Equal(0).When(ta => ta.Outflow > 0);
            RuleFor(ta => ta.Outflow).Equal(0).When(ta => ta.Inflow > 0);

            RuleFor(ta => ta.Envelope).NotNull();
            RuleFor(ta => ta.Envelope.Id).NotEmpty();
        }
    }
}