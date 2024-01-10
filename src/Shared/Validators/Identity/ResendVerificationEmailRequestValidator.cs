using FluentValidation;
using Spenses.Shared.Models.Identity;

namespace Spenses.Shared.Validators.Identity;

public class ResendVerificationEmailRequestValidator : AbstractValidator<ResendVerificationEmailRequest>
{
    public ResendVerificationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
    }
}
