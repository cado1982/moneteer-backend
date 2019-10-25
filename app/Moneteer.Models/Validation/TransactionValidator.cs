using FluentValidation;
using System.Linq;

namespace Moneteer.Models.Validation
{
    public class TransactionValidator : AbstractValidator<Transaction>
    {
        public TransactionValidator()
        {
            RuleFor(t => t.Account).NotNull();
            RuleFor(t => t.Account.Id).NotEmpty();
            RuleFor(t => t.Assignments).NotNull().NotEmpty();
            RuleFor(t => t.Description).Length(0, 500);
            RuleFor(t => t.IsCleared).Equal(true).When(t => t.IsReconciled).WithMessage("Reconciled transactions must also be cleared");
            RuleFor(t => t.Assignments).Must(a => !a.GroupBy(r => r.Envelope.Id).Any(g => g.Count() > 1)); // No duplicate envelopes
            RuleFor(t => t.Payee).Null().When(t => t.TargetAccount != null);
            RuleFor(t => t.TargetAccount).Null().When(t => t.Payee != null);
            RuleFor(t => t.TargetAccount).Must((t, ta) => !(ta == null && t.Payee == null));
            RuleFor(t => t.TargetAccount).Must((t, ta) => !(ta != null && t.Payee != null));

            RuleForEach(t => t.Assignments).SetValidator(new TransactionAssignmentValidator());
        }
    }
}