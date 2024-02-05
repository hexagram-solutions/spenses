using FluentValidation.TestHelper;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Validators.Members;

namespace Spenses.Shared.Tests.Validators.Members;

public class CreateMemberPropertiesValidatorTests
{
    private readonly CreateMemberPropertiesValidator _validator = new();

    [Fact]
    public void Contact_email_must_be_set_when_should_invite_is_true()
    {
        var model = new CreateMemberProperties { ShouldInvite = true };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.ContactEmail);

        _validator.TestValidate(model with { ContactEmail = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.ContactEmail);

        _validator.TestValidate(model with { ContactEmail = "foobar" })
            .ShouldHaveValidationErrorFor(x => x.ContactEmail);

        _validator.TestValidate(model with { ShouldInvite = false, ContactEmail = null })
            .ShouldNotHaveValidationErrorFor(x => x.ContactEmail);
    }
}
