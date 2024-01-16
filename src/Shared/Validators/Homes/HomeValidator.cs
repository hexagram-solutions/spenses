using FluentValidation;
using Spenses.Shared.Models.Homes;

namespace Spenses.Shared.Validators.Homes;

public class HomeValidator : AbstractValidator<Home>
{
    public HomeValidator()
    {
        Include(new HomePropertiesValidator());
    }
}
