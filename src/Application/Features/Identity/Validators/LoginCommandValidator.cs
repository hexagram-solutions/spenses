using FluentValidation;
using Spenses.Application.Features.Identity.Requests;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Application.Features.Identity.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new LoginRequestValidator());
    }
}
