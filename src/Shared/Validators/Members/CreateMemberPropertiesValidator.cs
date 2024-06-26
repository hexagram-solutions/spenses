using FluentValidation;
using Spenses.Shared.Models.Members;

namespace Spenses.Shared.Validators.Members;

public class CreateMemberPropertiesValidator : AbstractValidator<CreateMemberProperties>
{
    public CreateMemberPropertiesValidator()
    {
        Include(new MemberPropertiesValidator());

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .When(x => x.ShouldInvite)
            .WithMessage("A contact email must be set to invite this member to the home.");
    }
}
