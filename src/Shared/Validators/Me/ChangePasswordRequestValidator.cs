using FluentValidation;
using Spenses.Shared.Models.Me;

namespace Spenses.Shared.Validators.Me;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .MinimumLength(8);
    }
}
