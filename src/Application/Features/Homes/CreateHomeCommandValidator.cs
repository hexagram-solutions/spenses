using FluentValidation;

namespace Spenses.Application.Features.Homes;

public class CreateHomeCommandValidator : AbstractValidator<CreateHomeCommand>
{
    public CreateHomeCommandValidator()
    {
        RuleFor(x => x.Props.Name)
            .NotEmpty();
    }
}
