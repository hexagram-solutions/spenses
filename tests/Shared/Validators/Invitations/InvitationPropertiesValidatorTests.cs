using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Spenses.Shared.Models.Invitations;
using Spenses.Shared.Validators.Identity;
using Spenses.Shared.Validators.Invitations;

namespace Spenses.Shared.Tests.Validators.Invitations;

public class InvitationPropertiesValidatorTests
{
    private readonly InvitationPropertiesValidator _validator = new();

    [Fact]
    public void Email_must_be_valid()
    {
        var model = new InvitationProperties { Email = string.Empty };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "foobar"})
            .ShouldHaveValidationErrorFor(x => x.Email);

        _validator.TestValidate(model with { Email = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Member_id_must_not_be_empty_when_set()
    {
        var model = new InvitationProperties { MemberId = Guid.Empty };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MemberId);

        _validator.TestValidate(model with { MemberId = Guid.NewGuid() })
            .ShouldNotHaveValidationErrorFor(x => x.MemberId);

        _validator.TestValidate(model with { MemberId = null })
            .ShouldNotHaveValidationErrorFor(x => x.MemberId);
    }
}
