using FluentValidation.TestHelper;
using Spenses.Shared.Models.Members;
using Spenses.Shared.Validators.Members;

namespace Spenses.Shared.Tests.Validators.Members;

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
    public void Contact_email_must_be_valid_when_set()
    {
        var model = new MemberProperties { ContactEmail = null };

        _validator.TestValidate(model)
            .ShouldNotHaveValidationErrorFor(x => x.ContactEmail);

        _validator.TestValidate(model with { ContactEmail = "foobar" })
            .ShouldHaveValidationErrorFor(x => x.ContactEmail);

        _validator.TestValidate(model with { ContactEmail = "george@vandelayindustries.com" })
            .ShouldNotHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void Split_percentage_must_be_within_acceptable_range()
    {
        var model = new MemberProperties();

        _validator.TestValidate(model with { DefaultSplitPercentage = -0.1m })
            .ShouldHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 1.1m })
            .ShouldHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 0.55555m })
            .ShouldHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 0 })
            .ShouldNotHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 1.00m })
            .ShouldNotHaveValidationErrorFor(x => x.DefaultSplitPercentage);

        _validator.TestValidate(model with { DefaultSplitPercentage = 0.5555m })
            .ShouldNotHaveValidationErrorFor(x => x.DefaultSplitPercentage);
    }
}
