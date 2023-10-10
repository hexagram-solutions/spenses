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

        RuleFor(x => x.IncurredByMemberId)
            .NotEmpty();

        RuleFor(x => x.Tags)
            .NotEmpty()
            .Must(tags => tags.Select(t => t.ToLower()).Distinct().Count() == tags.Length)
            .WithMessage("Tags must be unique, ignoring case");
    }
}
