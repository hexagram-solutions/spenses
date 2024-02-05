using FluentValidation;
using Spenses.Shared.Models.Me;

namespace Spenses.Shared.Validators.Me;

public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequest>
{
    public ChangeEmailRequestValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();
    }
}
