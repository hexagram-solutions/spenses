using FluentValidation;
using Spenses.Application.Features.ExpenseCategories.Requests;

namespace Spenses.Application.Features.ExpenseCategories.Validators;

public class UpdateExpenseCategoryCommandValidator : AbstractValidator<UpdateExpenseCategoryCommand>
{
    public UpdateExpenseCategoryCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new ExpenseCategoryPropertiesValidator());
    }
}
