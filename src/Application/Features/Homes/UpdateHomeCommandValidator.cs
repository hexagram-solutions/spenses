using FluentValidation;

namespace Spenses.Application.Features.Homes;

public class UpdateHomeCommandValidator : AbstractValidator<UpdateHomeCommand>
{
    public UpdateHomeCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new HomePropertiesValidator());
    }
}
