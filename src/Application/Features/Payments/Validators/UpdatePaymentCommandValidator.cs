using FluentValidation;
using Spenses.Application.Features.Payments.Requests;
using Spenses.Shared.Validators.Payments;

namespace Spenses.Application.Features.Payments.Validators;

public class UpdatePaymentCommandValidator : AbstractValidator<UpdatePaymentCommand>
{
    public UpdatePaymentCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new PaymentPropertiesValidator());
    }
}
