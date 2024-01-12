using FluentValidation.TestHelper;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Shared.Tests.Validators.Identity;

public class ForgotPasswordRequestValidatorTests
{
    private readonly ForgotPasswordRequestValidator _validator = new();

    [Fact]
    public void Email_must_be_valid_email_address()
    {
        _validator.TestValidate(new ForgotPasswordRequest { Email = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ForgotPasswordRequest { Email = "@" })
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ForgotPasswordRequest { Email = "george@" })
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ForgotPasswordRequest { Email = "@vandelayindustries.com" })
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ForgotPasswordRequest { Email = string.Empty })
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}
