using FluentValidation.TestHelper;
using Spenses.Shared.Models.Me;
using Spenses.Shared.Validators.Me;

namespace Spenses.Shared.Tests.Validators.Me;

public class ChangeEmailRequestValidatorTests
{
    private readonly ChangeEmailRequestValidator _validator = new();

    [Fact]
    public void NewEmail_must_be_valid_email()
    {
        _validator.TestValidate(new ChangeEmailRequest { NewEmail = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(new ChangeEmailRequest { NewEmail = "@" })
            .ShouldHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(new ChangeEmailRequest { NewEmail = "george@" })
            .ShouldHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(new ChangeEmailRequest { NewEmail = "@vandelayindustries.com" })
            .ShouldHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(new ChangeEmailRequest { NewEmail = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.NewEmail);
    }
}
