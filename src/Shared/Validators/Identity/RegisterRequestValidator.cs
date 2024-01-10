using FluentValidation;
using Spenses.Shared.Models.Identity;

namespace Spenses.Shared.Validators.Identity;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(10);
    }
}
