using FluentValidation;
using Spenses.Application.Features.Invitations.Requests;
using Spenses.Shared.Validators.Invitations;

namespace Spenses.Application.Features.Invitations.Validators;

public class CreateInvitationCommandValidator : AbstractValidator<CreateInvitationCommand>
{
    public CreateInvitationCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new InvitationPropertiesValidator());
    }
}
