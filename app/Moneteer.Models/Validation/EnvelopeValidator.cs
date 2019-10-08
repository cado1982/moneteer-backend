using FluentValidation;

namespace Moneteer.Models.Validation
{
    public class EnvelopeValidator : AbstractValidator<Envelope>
    {
        public EnvelopeValidator()
        {
            RuleFor(e => e.Name).NotNull().MinimumLength(1).MaximumLength(250);

            RuleFor(e => e.EnvelopeCategory).NotNull();
            RuleFor(e => e.EnvelopeCategory.Id).NotEmpty();
        }
    }
}