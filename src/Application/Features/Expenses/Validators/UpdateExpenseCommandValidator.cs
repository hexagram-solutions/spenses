using FluentValidation;
using Spenses.Application.Features.Expenses.Requests;
using Spenses.Shared.Validators.Expenses;

namespace Spenses.Application.Features.Expenses.Validators;

public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
{
    public UpdateExpenseCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new ExpensePropertiesValidator());
    }
}
