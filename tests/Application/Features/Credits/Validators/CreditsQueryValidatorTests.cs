using FluentValidation.TestHelper;
using Spenses.Application.Features.Credits.Requests;
using Spenses.Application.Features.Credits.Validators;
using Spenses.Application.Models.Credits;
using Spenses.Application.Tests.Features.Common.Validators;

namespace Spenses.Application.Tests.Features.Credits.Validators;

public class CreditsQueryValidatorTests : PagedQueryValidatorTests<CreditDigest>
{
    private readonly CreditsQueryValidator _validator = new();

    [Fact]
    public void Min_date_must_be_less_than_or_equal_to_max_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new CreditsQuery(Guid.NewGuid()) { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MinDate);
    }

    [Fact]
    public void Max_date_must_be_greater_or_equal_to_min_date()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new CreditsQuery(Guid.NewGuid()) { MinDate = today.AddDays(1), MaxDate = today };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.MaxDate);
    }

    [Fact]
    public void Min_and_max_dates_can_be_equal()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var model = new CreditsQuery(Guid.NewGuid()) { MinDate = today, MaxDate = today };

        var validationResult = _validator.TestValidate(model);

        validationResult.ShouldNotHaveValidationErrorFor(x => x.MinDate);
        validationResult.ShouldNotHaveValidationErrorFor(x => x.MaxDate);
    }
}
