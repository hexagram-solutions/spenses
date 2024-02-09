using FluentValidation.TestHelper;
using Spenses.Shared.Models.Payments;
using Spenses.Shared.Tests.Validators.Common;
using Spenses.Shared.Validators.Payments;

namespace Spenses.Shared.Tests.Validators.Payments;

public class FilteredPaymentsQueryValidatorTests : PagedQueryValidatorTests<PaymentDigest>
{
    private readonly FilteredPaymentsQueryValidator _validator = new();

    [Fact]
    public void Min_date_is_required()
    {
        var model = new FilteredPaymentsQuery();

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MinDate);

        _validator.TestValidate(model with { MinDate = DateOnly.MinValue })
            .ShouldHaveValidationErrorFor(x => x.MinDate);
    }

    [Fact]
    public void Min_date_must_be_less_than_or_equal_to_max_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new FilteredPaymentsQuery { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MinDate);
    }

    [Fact]
    public void Max_date_is_required()
    {
        var model = new FilteredPaymentsQuery();

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MaxDate);

        _validator.TestValidate(model with { MaxDate = DateOnly.MinValue })
            .ShouldHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Max_date_must_be_greater_or_equal_to_min_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new FilteredPaymentsQuery { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Min_and_max_dates_can_be_equal()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new FilteredPaymentsQuery { MinDate = today, MaxDate = today };

        var validationResult = _validator.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.MinDate);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.MaxDate);
    }
}
