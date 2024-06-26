using FluentValidation;
using Spenses.Shared.Models.Expenses;

namespace Spenses.Shared.Validators.Expenses;

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
            .Must(tags => tags.Select(t => t.ToLower()).Distinct().Count() == tags.Count)
            .WithMessage("Tags must be unique, ignoring case");
    }
}
