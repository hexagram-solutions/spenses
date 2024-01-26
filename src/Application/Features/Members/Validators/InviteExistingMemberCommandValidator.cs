using FluentValidation;
using Spenses.Application.Features.Members.Requests;
using Spenses.Shared.Validators.Invitations;

namespace Spenses.Application.Features.Members.Validators;

public class InviteExistingMemberCommandValidator : AbstractValidator<InviteExistingMemberCommand>
{
    public InviteExistingMemberCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new InvitationPropertiesValidator());
    }
}
