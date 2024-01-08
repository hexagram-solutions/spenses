using FluentValidation.TestHelper;
using Spenses.Application.Features.ExpenseCategories.Validators;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Application.Tests.Features.ExpenseCategories.Validators;

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
