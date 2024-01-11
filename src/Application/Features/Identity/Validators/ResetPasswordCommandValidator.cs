using FluentValidation;
using Spenses.Application.Features.Identity.Requests;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Application.Features.Identity.Validators;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new ResetPasswordRequestValidator());
    }
}
