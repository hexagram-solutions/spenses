using FluentValidation;
using Spenses.Application.Features.Authentication.Requests;
using Spenses.Shared.Validators.Authentication;

namespace Spenses.Application.Features.Authentication.Validators;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new LoginRequestValidator());
    }
}
