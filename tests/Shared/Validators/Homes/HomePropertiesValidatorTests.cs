using FluentValidation.TestHelper;
using Spenses.Shared.Models.Homes;
using Spenses.Shared.Validators.Homes;

namespace Spenses.Shared.Tests.Validators.Homes;

public class HomePropertiesValidatorTests
{
    private readonly HomePropertiesValidator _validator = new();

    [Fact]
    public void Name_must_have_value()
    {
        var model = new HomeProperties();

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Name);

        _validator.TestValidate(model with { Name = "Valhalla" })
            .ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
