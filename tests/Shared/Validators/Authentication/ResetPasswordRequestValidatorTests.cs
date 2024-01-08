using FluentValidation.TestHelper;
using Spenses.Shared.Models.Authentication;
using Spenses.Shared.Validators.Authentication;

namespace Spenses.Shared.Tests.Validators.Authentication;

public class ResetPasswordRequestValidatorTests
{
    private readonly ResetPasswordRequestValidator _validator = new();

    [Fact]
    public void Email_must_be_valid_email_address()
    {
        var model = new ResetPasswordRequest(string.Empty, "foo", "hunter2");

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
        var model = new ResetPasswordRequest("george@vandelayindustries.com", string.Empty, "hunter2");

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.ResetCode);

        _validator.TestValidate(model with { ResetCode = "foo" })
            .ShouldNotHaveValidationErrorFor(x => x.ResetCode);
    }

    [Fact]
    public void New_password_is_required()
    {
        var model = new ResetPasswordRequest("george@vandelayindustries.com", "foo", string.Empty);

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.NewPassword);

        _validator.TestValidate(model with { NewPassword = "hunter2" })
            .ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }
}
