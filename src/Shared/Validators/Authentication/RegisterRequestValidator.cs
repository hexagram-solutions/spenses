using FluentValidation;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Shared.Validators.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(12);
    }
}
