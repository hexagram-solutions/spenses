using FluentValidation;
using Spenses.Application.Models;

namespace Spenses.Application.Features.Homes.Members;

public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new MemberPropertiesValidator());
    }
}

public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new MemberPropertiesValidator());
    }
}

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
