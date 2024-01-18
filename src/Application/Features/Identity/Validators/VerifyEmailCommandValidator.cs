using FluentValidation;
using Spenses.Application.Features.Identity.Requests;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Application.Features.Identity.Validators;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new VerifyEmailRequestValidator());
    }
}
