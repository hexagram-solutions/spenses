using FluentValidation;
using Spenses.Application.Models;

namespace Spenses.Application.Features.Homes;

public class CreateHomeCommandValidator : AbstractValidator<CreateHomeCommand>
{
    public CreateHomeCommandValidator()
    {
        RuleFor(x => x.Props.Name)
            .NotEmpty();
    }
}

public class HomePropertiesValidator : AbstractValidator<HomeProperties>
{
    public HomePropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
