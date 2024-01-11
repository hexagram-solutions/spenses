using FluentValidation;
using Spenses.Application.Features.Identity.Requests;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Application.Features.Identity.Validators;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new RegisterRequestValidator());
    }
}
