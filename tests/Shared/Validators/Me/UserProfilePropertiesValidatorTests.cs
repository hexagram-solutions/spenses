using FluentValidation.TestHelper;
using Spenses.Shared.Models.Me;
using Spenses.Shared.Validators.Me;

namespace Spenses.Shared.Tests.Validators.Me;

public class UserProfilePropertiesValidatorTests
{
    private readonly UserProfilePropertiesValidator _validator = new();

    [Fact]
    public void DisplayName_is_required()
    {
        _validator.TestValidate(new UserProfileProperties { DisplayName = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.DisplayName);

        _validator.TestValidate(new UserProfileProperties { DisplayName = "foo" })
            .ShouldNotHaveValidationErrorFor(x => x.DisplayName);
    }
}
