using FluentValidation;

namespace Moneteer.Models.Validation
{
    public class TransactionValidator : AbstractValidator<Transaction>
    {
        public TransactionValidator()
        {
            RuleFor(t => t.Account).NotNull();
            RuleFor(t => t.Account.Id).NotEmpty();
            RuleFor(t => t.Assignments).NotNull();
            RuleFor(t => t.Assignments).NotEmpty();
            RuleFor(t => t.Description).Length(0, 500);
            RuleFor(t => t.IsCleared).Equal(true).When(t => t.IsReconciled).Equal(true).WithMessage("Reconciled transactions must also be cleared");

            RuleForEach(t => t.Assignments).SetValidator(new TransactionAssignmentValidator());
        }
    }

    public class TransactionAssignmentValidator : AbstractValidator<TransactionAssignment>
    {
        public TransactionAssignmentValidator()
        {
            // Inflow and outflow not negative
            RuleFor(ta => ta.Inflow).GreaterThanOrEqualTo(0);
            RuleFor(ta => ta.Outflow).GreaterThanOrEqualTo(0);

            RuleFor(ta => ta.Inflow).Equal(0).When(ta => ta.Outflow > 0);
            RuleFor(ta => ta.Outflow).Equal(0).When(ta => ta.Inflow > 0);

            RuleFor(ta => ta.Envelope).NotNull();
            RuleFor(ta => ta.Envelope.Id).NotEmpty();
        }
    }
}