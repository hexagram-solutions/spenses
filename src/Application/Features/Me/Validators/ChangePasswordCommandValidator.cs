using FluentValidation;
using Spenses.Application.Features.Me.Requests;
using Spenses.Shared.Validators.Me;

namespace Spenses.Application.Features.Me.Validators;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new ChangePasswordRequestValidator());
    }
}
