using FluentValidation;
using Spenses.Shared.Models.Homes;

namespace Spenses.Application.Features.Homes.Validators;

public class HomePropertiesValidator : AbstractValidator<HomeProperties>
{
    public HomePropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
