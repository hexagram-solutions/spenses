using FluentValidation;
using Spenses.Application.Features.Credits.Requests;

namespace Spenses.Application.Features.Credits.Validators;

public class CreateCreditCommandValidator : AbstractValidator<CreateCreditCommand>
{
    public CreateCreditCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new CreditPropertiesValidator());
    }
}
