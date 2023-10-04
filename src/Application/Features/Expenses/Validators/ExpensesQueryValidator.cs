using FluentValidation;
using Spenses.Application.Features.Common.Validators;
using Spenses.Application.Features.Expenses.Requests;
using Spenses.Application.Models.Expenses;

namespace Spenses.Application.Features.Expenses.Validators;

public class ExpensesQueryValidator : AbstractValidator<ExpensesQuery>
{
    public ExpensesQueryValidator()
    {
        Include(new PagedQueryValidator<ExpenseDigest>());

        RuleFor(x => x.MinDate)
            .LessThanOrEqualTo(x => x.MaxDate)
            .When(x => x.MaxDate.HasValue);

        RuleFor(x => x.MaxDate)
            .GreaterThanOrEqualTo(x => x.MinDate)
            .When(x => x.MinDate.HasValue);

        RuleFor(x => x.Tags)
            .Must(x => x?.Distinct().Count() == x?.Length)
            .WithMessage("Tags must be unique")
            .ForEach(x => x.NotEmpty());
    }
}
