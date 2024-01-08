using FluentValidation;
using Spenses.Shared.Models.Members;

namespace Spenses.Application.Features.Members.Validators;

public class MemberPropertiesValidator : AbstractValidator<MemberProperties>
{
    public MemberPropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.ContactEmail)
            .EmailAddress();

        RuleFor(x => x.DefaultSplitPercentage)
            .PrecisionScale(5, 4, true)
            .InclusiveBetween(0.00m, 1.00m);
    }
}
