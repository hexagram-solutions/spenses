using FluentValidation;
using Spenses.Application.Models.Members;

namespace Spenses.Application.Features.Members.Validators;

public class MemberPropertiesValidator : AbstractValidator<MemberProperties>
{
    public MemberPropertiesValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.SplitPercentage)
            .InclusiveBetween(0d, 1d);
    }
}
