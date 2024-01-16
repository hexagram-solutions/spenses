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
            .LessThanOrEqualTo(x => x.MaxDate)
            .When(x => x.MaxDate.HasValue);

        RuleFor(x => x.MaxDate)
            .GreaterThanOrEqualTo(x => x.MinDate)
            .When(x => x.MinDate.HasValue);

        RuleFor(x => x.Tags)
            .Must(x =>
            {
                var tagsArray = x?.ToArray() ?? Array.Empty<string>();
                return tagsArray.Distinct().Count() == tagsArray.Length;
            })
            .WithMessage("Tags must be unique")
            .ForEach(x => x.NotEmpty());
    }
}
