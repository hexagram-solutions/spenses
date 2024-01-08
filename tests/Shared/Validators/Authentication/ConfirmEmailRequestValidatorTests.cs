using FluentValidation.TestHelper;
using Spenses.Shared.Models.Authentication;
using Spenses.Shared.Validators.Authentication;

namespace Spenses.Shared.Tests.Validators.Authentication;

public class ConfirmEmailRequestValidatorTests
{
    private readonly ConfirmEmailRequestValidator _validator = new();

    [Fact]
    public void User_id_is_required()
    {
        var model = new ConfirmEmailRequest(string.Empty, "foo");

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.UserId);

        _validator.TestValidate(model with { UserId = "bar" })
            .ShouldNotHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Code_is_required()
    {
        var model = new ConfirmEmailRequest("foo", string.Empty);

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Code);

        _validator.TestValidate(model with { Code = "bar" })
            .ShouldNotHaveValidationErrorFor(x => x.Code);
    }
}