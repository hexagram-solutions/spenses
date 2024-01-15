using FluentValidation;
using Spenses.Shared.Models.Me;

namespace Spenses.Shared.Validators.Me;

public class UserProfilePropertiesValidator : AbstractValidator<UserProfileProperties>
{
    public UserProfilePropertiesValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty();
    }
}
