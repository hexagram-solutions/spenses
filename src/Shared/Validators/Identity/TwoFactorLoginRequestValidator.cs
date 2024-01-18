using FluentValidation;
using Spenses.Shared.Models.Identity;

namespace Spenses.Shared.Validators.Identity;

public class TwoFactorLoginRequestValidator : AbstractValidator<TwoFactorLoginRequest>
{
    public TwoFactorLoginRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .When(x => string.IsNullOrEmpty(x.RecoveryCode));

        RuleFor(x => x.RecoveryCode)
            .NotEmpty()
            .When(x => string.IsNullOrEmpty(x.Code));
    }
}
