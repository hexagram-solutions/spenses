using FluentValidation.TestHelper;
using Spenses.Shared.Models.ExpenseCategories;
using Spenses.Shared.Validators.ExpenseCategories;

namespace Spenses.Shared.Tests.Validators.ExpenseCategories;

public class ExpenseCategoryPropertiesValidatorTests
{

    private readonly ExpenseCategoryPropertiesValidator _validator = new();

    [Fact]
    public void Name_must_have_value()
    {
        var model = new ExpenseCategoryProperties();

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);

        _validator.TestValidate(model with { Name = "gubbins" })
            .ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
