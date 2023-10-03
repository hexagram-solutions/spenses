using FluentValidation;
using Spenses.Application.Models;

namespace Spenses.Application.Features.Homes.Validators;

public class HomePropertiesValidator : AbstractValidator<HomeProperties>
{
    public HomePropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
