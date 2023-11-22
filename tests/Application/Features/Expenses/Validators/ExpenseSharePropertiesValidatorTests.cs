using FluentValidation.TestHelper;
using Spenses.Application.Features.Expenses.Validators;
using Spenses.Application.Models.Expenses;

namespace Spenses.Application.Tests.Features.Expenses.Validators;

public class ExpenseSharePropertiesValidatorTests
{
    private readonly ExpenseSharePropertiesValidator _validator = new();

    [Fact]
    public void Owed_amount_must_be_within_valid_range()
    {
        var model = new ExpenseShareProperties();

        _validator.TestValidate(model with { OwedAmount = -1.00m })
            .ShouldHaveValidationErrorFor(x => x.OwedAmount);

        _validator.TestValidate(model with { OwedAmount = 0.00m })
            .ShouldHaveValidationErrorFor(x => x.OwedAmount);

        _validator.TestValidate(model with { OwedAmount = 1_000_000.00m })
            .ShouldHaveValidationErrorFor(x => x.OwedAmount);

        _validator.TestValidate(model with { OwedAmount = 0.01m })
            .ShouldNotHaveValidationErrorFor(x => x.OwedAmount);

        _validator.TestValidate(model with { OwedAmount = 999_999.99m })
            .ShouldNotHaveValidationErrorFor(x => x.OwedAmount);
    }

    [Fact]
    public void Owed_amount_may_have_up_to_two_decimal_places()
    {
        var model = new ExpenseShareProperties();

        _validator.TestValidate(model with { OwedAmount = 1.111m })
            .ShouldHaveValidationErrorFor(x => x.OwedAmount);

        _validator.TestValidate(model with { OwedAmount = 1.1000m })
            .ShouldHaveValidationErrorFor(x => x.OwedAmount);

        _validator.TestValidate(model with { OwedAmount = 1.1m })
            .ShouldNotHaveValidationErrorFor(x => x.OwedAmount);

        _validator.TestValidate(model with { OwedAmount = 1m })
            .ShouldNotHaveValidationErrorFor(x => x.OwedAmount);
    }

    [Fact]
    public void Owed_by_member_id_must_have_value()
    {
        var model = new ExpenseShareProperties();

        _validator.TestValidate(model with { OwedByMemberId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.OwedByMemberId);

        _validator.TestValidate(model with { OwedByMemberId = Guid.NewGuid() })
            .ShouldNotHaveValidationErrorFor(x => x.OwedByMemberId);
    }
}
