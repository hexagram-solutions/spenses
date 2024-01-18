using FluentValidation;
using Spenses.Shared.Models.Identity;

namespace Spenses.Shared.Validators.Identity;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
