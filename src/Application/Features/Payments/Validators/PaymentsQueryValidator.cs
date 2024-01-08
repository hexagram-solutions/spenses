using FluentValidation;
using Spenses.Application.Features.Common.Validators;
using Spenses.Application.Features.Payments.Requests;
using Spenses.Shared.Models.Payments;

namespace Spenses.Application.Features.Payments.Validators;

public class PaymentsQueryValidator : AbstractValidator<PaymentsQuery>
{
    public PaymentsQueryValidator()
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
