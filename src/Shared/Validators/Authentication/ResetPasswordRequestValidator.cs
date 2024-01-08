using FluentValidation;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Shared.Validators.Authentication;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.ResetCode)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty();
    }
}
