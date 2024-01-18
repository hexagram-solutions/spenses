using FluentValidation;
using Spenses.Application.Features.Identity.Requests;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Application.Features.Identity.Validators;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new ForgotPasswordRequestValidator());
    }
}
