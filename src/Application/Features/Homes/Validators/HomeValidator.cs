using FluentValidation;
using Spenses.Shared.Models.Homes;

namespace Spenses.Application.Features.Homes.Validators;

public class HomeValidator : AbstractValidator<Home>
{
    public HomeValidator()
    {
        Include(new HomePropertiesValidator());
    }
}
