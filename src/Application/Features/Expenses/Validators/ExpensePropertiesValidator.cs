using FluentValidation;
using Spenses.Application.Models.Expenses;

namespace Spenses.Application.Features.Expenses.Validators;

public class ExpensePropertiesValidator : AbstractValidator<ExpenseProperties>
{
    public ExpensePropertiesValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .PrecisionScale(8, 2, false)
            .InclusiveBetween(0.01m, 999_999.99m);

        RuleFor(x => x.PaidByMemberId)
            .NotEmpty();

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Tags)
            .Must(tags => tags.Select(t => t.ToLower()).Distinct().Count() == tags.Length)
            .WithMessage("Tags must be unique, ignoring case");

        RuleFor(x => x.ExpenseShares)
            .NotEmpty()
            .Must((ep, _) => ep.ExpenseShares.Sum(x => x.OwedAmount) == ep.Amount)
                .WithMessage("The sum of all expense shares must be equivalent to the expense amount")
            .Must((ep, _) => ep.ExpenseShares.DistinctBy(es => es.OwedByMemberId).Count() == ep.ExpenseShares.Length)
                .WithMessage("Expense shares must not contain duplicate members.");

        RuleForEach(x => x.ExpenseShares)
            .SetValidator(new ExpenseSharePropertiesValidator());
    }
}
