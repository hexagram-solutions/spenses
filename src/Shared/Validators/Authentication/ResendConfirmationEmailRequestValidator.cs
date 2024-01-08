using FluentValidation;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Shared.Validators.Authentication;

public class ResendConfirmationEmailRequestValidator : AbstractValidator<ResendConfirmationEmailRequest>
{
    public ResendConfirmationEmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
    }
}
