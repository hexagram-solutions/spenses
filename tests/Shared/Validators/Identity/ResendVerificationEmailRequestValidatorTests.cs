using FluentValidation.TestHelper;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Shared.Tests.Validators.Identity;

public class ResendVerificationEmailRequestValidatorTests
{
    private readonly ResendVerificationEmailRequestValidator _validator = new();

    [Fact]
    public void Email_must_be_valid_email_address()
    {
        _validator.TestValidate(new ResendVerificationEmailRequest(string.Empty))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendVerificationEmailRequest(Email: "@"))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendVerificationEmailRequest(Email: "george@"))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendVerificationEmailRequest(Email: "@vandelayindustries.com"))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendVerificationEmailRequest(Email: "george@vandelayindustries.com"))
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}
