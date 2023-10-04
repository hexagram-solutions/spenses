using FluentValidation;
using Spenses.Application.Models.Credits;

namespace Spenses.Application.Features.Credits.Validators;

public class CreditPropertiesValidator : AbstractValidator<CreditProperties>
{
    public CreditPropertiesValidator()
    {
        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .PrecisionScale(8, 2, false)
            .InclusiveBetween(0.01m, 999_999.99m);

        RuleFor(x => x.PaidByMemberId)
            .NotEmpty();
    }
}
