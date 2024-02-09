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
            .NotEmpty()
            .LessThanOrEqualTo(x => x.MaxDate);

        RuleFor(x => x.MaxDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.MinDate);
    }
}
