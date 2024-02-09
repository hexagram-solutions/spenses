using FluentValidation.TestHelper;
using Spenses.Shared.Models.Expenses;
using Spenses.Shared.Tests.Validators.Common;
using Spenses.Shared.Validators.Expenses;

namespace Spenses.Shared.Tests.Validators.Expenses;

public class FilteredExpensesQueryValidatorTests : PagedQueryValidatorTests<ExpenseDigest>
{
    private readonly FilteredExpensesQueryValidator _validator = new();

    [Fact]
    public void Min_date_is_required()
    {
        var model = new FilteredExpensesQuery();

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MinDate);

        _validator.TestValidate(model with { MinDate = DateOnly.MinValue })
            .ShouldHaveValidationErrorFor(x => x.MinDate);
    }

    [Fact]
    public void Min_date_must_be_less_than_or_equal_to_max_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new FilteredExpensesQuery { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MinDate);
    }

    [Fact]
    public void Max_date_is_required()
    {
        var model = new FilteredExpensesQuery();

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MaxDate);

        _validator.TestValidate(model with { MaxDate = DateOnly.MinValue })
            .ShouldHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Max_date_must_be_greater_or_equal_to_min_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new FilteredExpensesQuery { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Min_and_max_dates_can_be_equal()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new FilteredExpensesQuery { MinDate = today, MaxDate = today };

        var validationResult = _validator.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.MinDate);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Tags_cannot_contain_empty_values()
    {
        var model = new FilteredExpensesQuery { Tags = new[] { "" } };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Tags);

        _validator.TestValidate(model with { Tags = new[] { "foo" } })
            .ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Tags_must_be_unique()
    {
        var model = new FilteredExpensesQuery { Tags = new[] { "foo", "foo" } };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Tags);

        _validator.TestValidate(model with { Tags = new[] { "foo", "bar" } })
            .ShouldNotHaveValidationErrorFor(x => x.Tags);
    }
}
