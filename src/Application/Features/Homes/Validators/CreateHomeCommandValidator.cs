using FluentValidation;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Shared.Validators.Homes;

namespace Spenses.Application.Features.Homes.Validators;

public class CreateHomeCommandValidator : AbstractValidator<CreateHomeCommand>
{
    public CreateHomeCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new HomePropertiesValidator());
    }
}
