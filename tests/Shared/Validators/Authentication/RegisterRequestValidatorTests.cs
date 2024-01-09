using FluentValidation.TestHelper;
using Spenses.Shared.Models.Authentication;
using Spenses.Shared.Validators.Authentication;

namespace Spenses.Shared.Tests.Validators.Authentication;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Email_must_be_valid_email_address()
    {
        var model = new RegisterRequest { Email = string.Empty, Password = "hunter2"};

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "@" })
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "george@" })
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "@vandelayindustries.com" })
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Password_must_be_at_least_12_characters_long()
    {
        var model = new RegisterRequest { Email = "george@vandelayindustries.com", Password = string.Empty };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Password);

        _validator.TestValidate(model with { Password = new string('a', 11) })
            .ShouldHaveValidationErrorFor(x => x.Password);

        _validator.TestValidate(model with { Password = new string('a', 12) })
            .ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}
