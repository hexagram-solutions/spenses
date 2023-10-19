using FluentValidation;
using Spenses.Application.Models.Members;

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
            .InclusiveBetween(0.00m, 1.00m);
    }
}
