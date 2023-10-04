using FluentValidation;
using Spenses.Application.Models.Homes;

namespace Spenses.Application.Features.Homes.Validators;

public class HomePropertiesValidator : AbstractValidator<HomeProperties>
{
    public HomePropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
