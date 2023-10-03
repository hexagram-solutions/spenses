using FluentValidation;
using Spenses.Application.Features.Members.Requests;

namespace Spenses.Application.Features.Members.Validators;

public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new MemberPropertiesValidator());
    }
}
