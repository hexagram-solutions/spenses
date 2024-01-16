using FluentValidation;
using Spenses.Shared.Models.Payments;
using Spenses.Shared.Validators.Common;

namespace Spenses.Shared.Validators.Payments;

public class FilteredPaymentsQueryValidator : AbstractValidator<FilteredPaymentsQuery>
{
    public FilteredPaymentsQueryValidator()
    {
        Include(new PagedQueryValidator<PaymentDigest>());

        RuleFor(x => x.MinDate)
            .LessThanOrEqualTo(x => x.MaxDate)
            .When(x => x.MaxDate.HasValue);

        RuleFor(x => x.MaxDate)
            .GreaterThanOrEqualTo(x => x.MinDate)
            .When(x => x.MinDate.HasValue);
    }
}
