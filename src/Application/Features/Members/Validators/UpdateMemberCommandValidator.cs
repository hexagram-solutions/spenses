using FluentValidation;
using Spenses.Application.Features.Members.Requests;
using Spenses.Shared.Validators.Members;

namespace Spenses.Application.Features.Members.Validators;

public class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new MemberPropertiesValidator());
    }
}
