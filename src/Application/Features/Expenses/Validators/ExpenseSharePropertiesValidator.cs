using FluentValidation;
using Spenses.Application.Models.Expenses;

namespace Spenses.Application.Features.Expenses.Validators;
public class ExpenseSharePropertiesValidator : AbstractValidator<ExpenseShareProperties>
{
    public ExpenseSharePropertiesValidator()
    {
        RuleFor(x => x.OwedByMemberId)
            .NotEmpty();

        RuleFor(x => x.OwedAmount)
            .PrecisionScale(8, 2, false)
            .InclusiveBetween(0.01m, 999_999.99m);
    }
}
