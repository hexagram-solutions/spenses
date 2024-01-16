using FluentValidation;
using Spenses.Application.Features.Payments.Requests;
using Spenses.Shared.Validators.Payments;

namespace Spenses.Application.Features.Payments.Validators;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new PaymentPropertiesValidator());
    }
}
