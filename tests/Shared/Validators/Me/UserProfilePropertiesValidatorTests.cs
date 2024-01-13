using FluentValidation.TestHelper;
using Spenses.Shared.Models.Me;
using Spenses.Shared.Validators.Me;

namespace Spenses.Shared.Tests.Validators.Me;

public class UserProfilePropertiesValidatorTests
{
    private readonly UserProfilePropertiesValidator _validator = new();

    [Fact]
    public void NickName_is_required()
    {
        _validator.TestValidate(new UserProfileProperties { NickName = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.NickName);

        _validator.TestValidate(new UserProfileProperties { NickName = "foo" })
            .ShouldNotHaveValidationErrorFor(x => x.NickName);
    }
}
