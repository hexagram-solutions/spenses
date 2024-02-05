using FluentValidation;
using Spenses.Shared.Models.Invitations;

namespace Spenses.Shared.Validators.Invitations;

public class InvitationPropertiesValidator : AbstractValidator<InvitationProperties>
{
    public InvitationPropertiesValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
