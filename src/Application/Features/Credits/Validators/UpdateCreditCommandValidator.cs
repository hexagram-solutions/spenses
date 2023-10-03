using FluentValidation;
using Spenses.Application.Features.Credits.Requests;

namespace Spenses.Application.Features.Credits.Validators;

public class UpdateCreditCommandValidator : AbstractValidator<UpdateCreditCommand>
{
    public UpdateCreditCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new CreditPropertiesValidator());
    }
}
