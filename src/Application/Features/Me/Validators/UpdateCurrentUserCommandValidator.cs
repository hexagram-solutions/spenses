using FluentValidation;
using Spenses.Application.Features.Me.Requests;
using Spenses.Shared.Validators.Me;

namespace Spenses.Application.Features.Me.Validators;

public class UpdateCurrentUserCommandValidator : AbstractValidator<UpdateCurrentUserCommand>
{
    public UpdateCurrentUserCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new UserProfilePropertiesValidator());
    }
}
