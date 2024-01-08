using FluentValidation;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Shared.Validators.Authentication;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
