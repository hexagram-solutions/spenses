using FluentValidation.TestHelper;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Shared.Tests.Validators.Identity;

public class VerifyEmailRequestValidatorTests
{
    private readonly VerifyEmailRequestValidator _validator = new();

    [Fact]
    public void User_id_is_required()
    {
        var model = new VerifyEmailRequest(string.Empty, "foo");

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.UserId);

        _validator.TestValidate(model with { UserId = "bar" })
            .ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Code_is_required()
    {
        var model = new VerifyEmailRequest("foo", string.Empty);

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Code);

        _validator.TestValidate(model with { Code = "bar" })
            .ShouldNotHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void New_email_must_be_valid_when_not_empty()
    {
        var model = new VerifyEmailRequest("foo", "bar");

        _validator.TestValidate(model)
            .ShouldNotHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(model with { NewEmail = string.Empty })
            .ShouldNotHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(model with { NewEmail = "@" })
            .ShouldHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(model with { NewEmail = "george@" })
            .ShouldHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(model with { NewEmail = "@vandelayindustries.com" })
            .ShouldHaveValidationErrorFor(x => x.NewEmail);

        _validator.TestValidate(model with { NewEmail = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.NewEmail);
    }
}
