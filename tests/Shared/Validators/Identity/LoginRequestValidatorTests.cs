using FluentValidation.TestHelper;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Validators.Identity;

namespace Spenses.Shared.Tests.Validators.Identity;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Fact]
    public void Email_is_required()
    {
        var model = new LoginRequest { Email = string.Empty, Password = "hunter2" };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Password_is_required()
    {
        var model = new LoginRequest { Email = "george@vandelayindustries.com", Password = string.Empty };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Password);

        _validator.TestValidate(model with { Password = "hunter2" })
            .ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}
