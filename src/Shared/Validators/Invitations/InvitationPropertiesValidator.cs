using FluentValidation;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Shared.Validators.Invitations;

public class InvitationPropertiesValidator : AbstractValidator<InvitationProperties>
{
    public InvitationPropertiesValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.MemberId)
            .Must(x => x != Guid.Empty)
            .When(x => x.MemberId is not null);
    }
}
