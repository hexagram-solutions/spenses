using FluentValidation;
using Spenses.Shared.Models.Expenses;
using Spenses.Shared.Validators.Common;

namespace Spenses.Shared.Validators.Expenses;

public class FilteredExpensesQueryValidator : AbstractValidator<FilteredExpensesQuery>
{
    public FilteredExpensesQueryValidator()
    {
        Include(new PagedQueryValidator<ExpenseDigest>());

        RuleFor(x => x.MinDate)
            .NotEmpty()
            .LessThanOrEqualTo(x => x.MaxDate);

        RuleFor(x => x.MaxDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.MinDate);

        RuleFor(x => x.Tags)
            .Must(x =>
            {
                var tagsArray = x?.ToArray() ?? [];
                return tagsArray.Distinct().Count() == tagsArray.Length;
            })
            .WithMessage("Tags must be unique")
            .ForEach(x => x.NotEmpty());
    }
}
