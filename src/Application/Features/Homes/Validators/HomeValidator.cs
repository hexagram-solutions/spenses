using FluentValidation;
using Spenses.Application.Models.Homes;

namespace Spenses.Application.Features.Homes.Validators;

public class HomeValidator : AbstractValidator<Home>
{
    public HomeValidator()
    {
        Include(new HomePropertiesValidator());
    }
}
