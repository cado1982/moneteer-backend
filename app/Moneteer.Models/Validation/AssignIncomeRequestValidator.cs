using FluentValidation;

namespace Moneteer.Models.Validation
{
    public class AssignIncomeRequestValidator : AbstractValidator<AssignIncomeRequest>
    {
        public AssignIncomeRequestValidator()
        {
            RuleFor(a => a.Assignments).NotNull();
            RuleFor(a => a.Assignments).NotEmpty();

            RuleForEach(a => a.Assignments).SetValidator(new AssignIncomeValidator());
        }
    }

    public class AssignIncomeValidator : AbstractValidator<AssignIncome>
    {
        public AssignIncomeValidator()
        {
            RuleFor(a => a.Envelope).NotNull();
            RuleFor(a => a.Envelope.Id).NotEmpty();

            RuleFor(a => a.Amount).GreaterThan(0);
        }
    }
}