using FluentValidation;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Shared.Validators.Authentication;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();
    }
}
