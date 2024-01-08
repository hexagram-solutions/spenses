using FluentValidation;
using Spenses.Shared.Models.ExpenseCategories;

namespace Spenses.Application.Features.ExpenseCategories.Validators;

public class ExpenseCategoryPropertiesValidator : AbstractValidator<ExpenseCategoryProperties>
{
    public ExpenseCategoryPropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
