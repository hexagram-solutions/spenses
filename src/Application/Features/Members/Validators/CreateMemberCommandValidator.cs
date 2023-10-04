using FluentValidation;
using Spenses.Application.Features.Members.Requests;

namespace Spenses.Application.Features.Members.Validators;

public class CreateMemberCommandValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberCommandValidator()
    {
        RuleFor(x => x.Props)
            .SetValidator(new MemberPropertiesValidator());
    }
}