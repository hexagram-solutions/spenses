using FluentValidation.TestHelper;
using Spenses.Application.Features.Homes.Validators;
using Spenses.Application.Models.Homes;

namespace Spenses.Application.Tests.Features.Homes.Validators;

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

    [Fact]
    public void Expense_period_must_be_in_enum()
    {
        var model = new HomeProperties { ExpensePeriod = (ExpensePeriod) 100 };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.ExpensePeriod);

        _validator.TestValidate(model with { ExpensePeriod = ExpensePeriod.Monthly })
            .ShouldNotHaveValidationErrorFor(x => x.ExpensePeriod);
    }
}
