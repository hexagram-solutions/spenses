using FluentValidation;
using Spenses.Application.Features.Homes.Requests;
using Spenses.Shared.Validators.Homes;

namespace Spenses.Application.Features.Homes.Validators;

public class UpdateHomeCommandValidator : AbstractValidator<UpdateHomeCommand>
{
    public UpdateHomeCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new HomePropertiesValidator());
    }
}
