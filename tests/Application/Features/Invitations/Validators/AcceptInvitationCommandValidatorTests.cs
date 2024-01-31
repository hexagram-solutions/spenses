using FluentValidation.TestHelper;
using Spenses.Application.Features.Invitations.Requests;
using Spenses.Application.Features.Invitations.Validators;

namespace Spenses.Application.Tests.Features.Invitations.Validators;

public class AcceptInvitationCommandValidatorTests
{
    private readonly AcceptInvitationCommandValidator _validator = new();

    [Fact]
    public void Token_must_not_be_empty()
    {
        _validator.TestValidate(new AcceptInvitationCommand(Guid.Empty))
            .ShouldHaveValidationErrorFor(x => x.InvitationId);

        _validator.TestValidate(new AcceptInvitationCommand(Guid.NewGuid()))
            .ShouldNotHaveValidationErrorFor(x => x.InvitationId);
    }
}
