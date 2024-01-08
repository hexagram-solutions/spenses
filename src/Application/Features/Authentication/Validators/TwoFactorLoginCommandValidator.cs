using FluentValidation;
using Spenses.Application.Features.Authentication.Requests;
using Spenses.Shared.Validators.Authentication;

namespace Spenses.Application.Features.Authentication.Validators;

public class TwoFactorLoginCommandValidator : AbstractValidator<TwoFactorLoginCommand>
{
    public TwoFactorLoginCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new TwoFactorLoginRequestValidator());
    }
}
