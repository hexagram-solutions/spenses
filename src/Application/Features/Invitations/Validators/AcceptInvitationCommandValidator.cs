using FluentValidation;
using Spenses.Application.Features.Invitations.Requests;

namespace Spenses.Application.Features.Invitations.Validators;

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}
