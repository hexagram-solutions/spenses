using FluentValidation;
using Spenses.Shared.Models.Identity;

namespace Spenses.Shared.Validators.Identity;

public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
