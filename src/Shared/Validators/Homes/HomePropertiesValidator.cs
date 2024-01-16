using FluentValidation;
using Spenses.Shared.Models.Homes;

namespace Spenses.Shared.Validators.Homes;

public class HomePropertiesValidator : AbstractValidator<HomeProperties>
{
    public HomePropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}
