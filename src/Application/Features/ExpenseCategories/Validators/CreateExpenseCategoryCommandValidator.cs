using FluentValidation;
using Spenses.Application.Features.ExpenseCategories.Requests;
using Spenses.Shared.Validators.ExpenseCategories;

namespace Spenses.Application.Features.ExpenseCategories.Validators;

public class CreateExpenseCategoryCommandValidator : AbstractValidator<CreateExpenseCategoryCommand>
{
    public CreateExpenseCategoryCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new ExpenseCategoryPropertiesValidator());
    }
}
