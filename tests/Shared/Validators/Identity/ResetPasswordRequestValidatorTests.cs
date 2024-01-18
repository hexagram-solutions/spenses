using FluentValidation.TestHelper;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Shared.Tests.Validators.Identity;

public class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public void Email_must_be_valid_email_address()
    {
        var model = new ResetPasswordRequest
        {
            Email = string.Empty,
            ResetCode = "foo",
            NewPassword = "hunter2"
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
    public void Reset_code_is_required()
    {
        var model = new ResetPasswordRequest
        {
            Email = "george@vandelayindustries.com",
            ResetCode = string.Empty,
            NewPassword = "hunter2"
        };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.ResetCode);

        _validator.TestValidate(model with { ResetCode = "foo" })
            .ShouldNotHaveValidationErrorFor(x => x.ResetCode);
    }

    [Fact]
    public void New_password_must_be_at_least_10_characters_long()
    {
        var model = new ResetPasswordRequest
        {
            Email = "george@vandelayindustries.com",
            ResetCode = "foo",
            NewPassword = string.Empty
        };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.NewPassword);

        _validator.TestValidate(model with { NewPassword = new string('a', 9) })
            .ShouldHaveValidationErrorFor(x => x.NewPassword);

        _validator.TestValidate(model with { NewPassword = new string('a', 10) })
            .ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }
}
