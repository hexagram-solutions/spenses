using FluentValidation.TestHelper;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Validators.Invitations;

namespace Spenses.Shared.Tests.Validators.Invitations;

public class InvitationPropertiesValidatorTests
{
    private readonly InvitationPropertiesValidator _validator = new();

    [Fact]
    public void Email_must_be_valid()
    {
        var model = new InvitationProperties { Email = string.Empty };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "foobar"})
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}
