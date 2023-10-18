using FluentValidation.TestHelper;
using Spenses.Application.Features.Members.Validators;
using Spenses.Application.Models.Members;

namespace Spenses.Application.Tests.Features.Members.Validators;

public class MemberPropertiesValidatorTests
{
    private readonly MemberPropertiesValidator _validator = new();

    [Fact]
    public void Name_must_have_value()
    {
        var model = new MemberProperties();

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Name);

        _validator.TestValidate(model with { Name = "George Costanza" })
            .ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Contact_email_must_be_valid()
    {
        var model = new MemberProperties { ContactEmail = "foobar" };

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.ContactEmail);

        _validator.TestValidate(model with { ContactEmail = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void Split_percentage_must_be_within_acceptable_range()
    {
        var model = new MemberProperties();

        _validator.TestValidate(model with { DefaultSplitPercentage = -0.1 })
            .ShouldHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 1.1 })
            .ShouldHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 0 })
            .ShouldNotHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 1 })
            .ShouldNotHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 0.5 })
            .ShouldNotHaveValidationErrorFor(x => x.DefaultSplitPercentage);
    }
}
