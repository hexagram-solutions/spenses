using FluentValidation;
using Spenses.Shared.Models.Members;

namespace Spenses.Application.Features.Members.Validators;

public class MemberValidator : AbstractValidator<Member>
{
    public MemberValidator()
    {
        Include(new MemberPropertiesValidator());
    }
}
