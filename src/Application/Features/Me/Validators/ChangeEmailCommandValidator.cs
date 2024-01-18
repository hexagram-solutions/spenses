using FluentValidation;
using Spenses.Application.Features.Me.Requests;
using Spenses.Shared.Validators.Me;

namespace Spenses.Application.Features.Me.Validators;

public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.Request)
            .SetValidator(new ChangeEmailRequestValidator());
    }
}
