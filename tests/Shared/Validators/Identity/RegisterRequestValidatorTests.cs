using FluentValidation.TestHelper;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Shared.Tests.Validators.Identity;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Email_must_be_valid_email_address()
    {
        var model = new RegisterRequest
        {
            Email = string.Empty,
            Password = "hunter2",
            NickName = "George Costanza"
        };

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
        var model = new RegisterRequest
        {
            Email = "george@vandelayindustries.com",
            Password = string.Empty,
            NickName = "George Costanza"
        };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Password);

        _validator.TestValidate(model with { Password = new string('a', 9) })
            .ShouldHaveValidationErrorFor(x => x.Password);

        _validator.TestValidate(model with { Password = new string('a', 10) })
            .ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Nick_name_is_required()
    {
        var model = new RegisterRequest
        {
            Email = "george@vandelayindustries.com",
            Password = "hunter2",
            NickName = string.Empty
        };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.NickName);

        _validator.TestValidate(model with { NickName = "George Costanza" })
            .ShouldNotHaveValidationErrorFor(x => x.NickName);
    }
}
