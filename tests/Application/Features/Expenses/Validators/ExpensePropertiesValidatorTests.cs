using FluentValidation.TestHelper;
using Spenses.Application.Features.Expenses.Validators;
using Spenses.Application.Models.Expenses;

namespace Spenses.Application.Tests.Features.Expenses.Validators;

public class ExpensePropertiesValidatorTests
{
    private readonly ExpensePropertiesValidator _validator = new();

    [Fact]
    public void Date_must_have_value()
    {
        var model = new ExpenseProperties();

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Date);

        _validator.TestValidate(model with { Date = DateOnly.MinValue })
            .ShouldHaveValidationErrorFor(x => x.Date);

        _validator.TestValidate(model with { Date = DateOnly.FromDateTime(DateTime.UtcNow) })
            .ShouldNotHaveValidationErrorFor(x => x.Date);
    }

    [Fact]
    public void Amount_must_have_value()
    {
        var model = new ExpenseProperties();

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 3.50m })
            .ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_must_be_within_valid_range()
    {
        var model = new ExpenseProperties();

        _validator.TestValidate(model with { Amount = -1.00m })
            .ShouldHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 0.00m })
            .ShouldHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 1_000_000.00m })
            .ShouldHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 0.01m })
            .ShouldNotHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 999_999.99m })
            .ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_may_have_up_to_two_decimal_places()
    {
        var model = new ExpenseProperties();

        _validator.TestValidate(model with { Amount = 1.111m })
            .ShouldHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 1.1000m })
            .ShouldHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 1.1m })
            .ShouldNotHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 1m })
            .ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Paid_by_member_id_must_have_value()
    {
        var model = new ExpenseProperties();

        _validator.TestValidate(model with { PaidByMemberId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.PaidByMemberId);

        _validator.TestValidate(model with { PaidByMemberId = Guid.NewGuid() })
            .ShouldNotHaveValidationErrorFor(x => x.PaidByMemberId);
    }

    [Fact]
    public void Tags_values_must_be_distinct()
    {
        var model = new ExpenseProperties { Tags = new[] { "foo", "Foo", "bar" } };

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Tags);

        _validator.TestValidate(model with { Tags = new[] { "foo", "bar", "baz" } })
            .ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Category_id_must_have_value()
    {
        var model = new ExpenseProperties();

        _validator.TestValidate(model with { CategoryId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.PaidByMemberId);

        _validator.TestValidate(model with { PaidByMemberId = Guid.NewGuid() })
            .ShouldNotHaveValidationErrorFor(x => x.PaidByMemberId);
    }
}
