using FluentValidation;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Shared.Validators.ExpenseCategories;

public class ExpenseCategoryPropertiesValidator : AbstractValidator<ExpenseCategoryProperties>
{
    public ExpenseCategoryPropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
