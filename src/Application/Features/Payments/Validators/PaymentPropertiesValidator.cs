using FluentValidation;
using Spenses.Application.Models.Payments;

namespace Spenses.Application.Features.Payments.Validators;

public class PaymentPropertiesValidator : AbstractValidator<PaymentProperties>
{
    public PaymentPropertiesValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .PrecisionScale(8, 2, false)
            .InclusiveBetween(0.01m, 999_999.99m);

        RuleFor(x => x.PaidByMemberId)
            .NotEmpty()
            .NotEqual(x => x.PaidToMemberId);

        RuleFor(x => x.PaidToMemberId)
            .NotEmpty()
            .NotEqual(x => x.PaidByMemberId);
    }
}
