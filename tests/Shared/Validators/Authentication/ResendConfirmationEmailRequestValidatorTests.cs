using FluentValidation.TestHelper;
using Spenses.Shared.Models.Authentication;
using Spenses.Shared.Validators.Authentication;

namespace Spenses.Shared.Tests.Validators.Authentication;

public class ResendConfirmationEmailRequestValidatorTests
{
    private readonly ResendConfirmationEmailRequestValidator _validator = new();

    [Fact]
    public void Email_must_be_valid_email_address()
    {
        _validator.TestValidate(new ResendConfirmationEmailRequest(string.Empty))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendConfirmationEmailRequest(Email: "@"))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendConfirmationEmailRequest(Email: "george@"))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendConfirmationEmailRequest(Email: "@vandelayindustries.com"))
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(new ResendConfirmationEmailRequest(Email: "george@vandelayindustries.com"))
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }
}
