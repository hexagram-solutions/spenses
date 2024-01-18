using FluentValidation.TestHelper;
using Spenses.Shared.Models.Me;
using Spenses.Shared.Validators.Me;

namespace Spenses.Shared.Tests.Validators.Me;

public class ChangePasswordRequestValidatorTests
{
    private readonly ChangePasswordRequestValidator _validator = new();

    [Fact]
    public void CurrentPassword_is_required()
    {
        var model = new ChangePasswordRequest
        {
            CurrentPassword = string.Empty,
            NewPassword = "hunter2",
        };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.CurrentPassword);

        _validator.TestValidate(model with { CurrentPassword = "hunter22" })
            .ShouldNotHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Fact]
    public void NewPassword_must_be_at_least_10_characters_long()
    {
        var model = new ChangePasswordRequest
        {
            CurrentPassword = "foo",
            NewPassword = string.Empty,
        };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.NewPassword);

        _validator.TestValidate(model with { NewPassword = new string('a', 7) })
            .ShouldHaveValidationErrorFor(x => x.NewPassword);

        _validator.TestValidate(model with { NewPassword = new string('a', 8) })
            .ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }
}
