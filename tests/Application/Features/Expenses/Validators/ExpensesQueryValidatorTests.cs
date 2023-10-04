using FluentValidation.TestHelper;
using Spenses.Application.Features.Expenses.Requests;
using Spenses.Application.Features.Expenses.Validators;
using Spenses.Application.Models.Expenses;
using Spenses.Application.Tests.Features.Common.Validators;

namespace Spenses.Application.Tests.Features.Expenses.Validators;

public class ExpensesQueryValidatorTests : PagedQueryValidatorTests<ExpenseDigest>
{
    private readonly ExpensesQueryValidator _validator = new();

    [Fact]
    public void Min_date_must_be_less_than_or_equal_to_max_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new ExpensesQuery(Guid.NewGuid()) { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MinDate);
    }

    [Fact]
    public void Max_date_must_be_greater_or_equal_to_min_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new ExpensesQuery(Guid.NewGuid()) { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Min_and_max_dates_can_be_equal()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new ExpensesQuery(Guid.NewGuid()) { MinDate = today, MaxDate = today };

        var validationResult = _validator.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.MinDate);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Tags_cannot_contain_empty_values()
    {
        var model = new ExpensesQuery(Guid.NewGuid()) { Tags = new[] { "" } };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Tags);

        _validator.TestValidate(model with { Tags = new[] { "foo" } })
            .ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Tags_must_be_unique()
    {
        var model = new ExpensesQuery(Guid.NewGuid()) { Tags = new[] { "foo", "foo" } };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Tags);

        _validator.TestValidate(model with { Tags = new[] { "foo", "bar" } })
            .ShouldNotHaveValidationErrorFor(x => x.Tags);
    }
}
