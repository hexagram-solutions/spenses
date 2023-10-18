using FluentValidation.TestHelper;
using Spenses.Application.Features.Payments.Validators;
using Spenses.Application.Models.Payments;

namespace Spenses.Application.Tests.Features.Payments.Validators;

public class PaymentPropertiesValidatorTests
{
    private readonly PaymentPropertiesValidator _validator = new();

    [Fact]
    public void Date_must_have_value()
    {
        var model = new PaymentProperties();

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Date);

        _validator.TestValidate(model with { Date = DateOnly.MinValue })
            .ShouldHaveValidationErrorFor(x => x.Date);

        _validator.TestValidate(model with { Date = DateOnly.FromDateTime(DateTime.UtcNow) })
            .ShouldNotHaveValidationErrorFor(x => x.Date);
    }

    [Fact]
    public void Amount_must_have_value()
    {
        var model = new PaymentProperties();

        _validator.TestValidate(model).ShouldHaveValidationErrorFor(x => x.Amount);

        _validator.TestValidate(model with { Amount = 3.50m })
            .ShouldNotHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Amount_must_be_within_valid_range()
    {
        var model = new PaymentProperties();

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
        var model = new PaymentProperties();

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
        var model = new PaymentProperties();

        _validator.TestValidate(model with { PaidByMemberId = Guid.Empty })
            .ShouldHaveValidationErrorFor(x => x.PaidByMemberId);

        _validator.TestValidate(model with { PaidByMemberId = Guid.NewGuid() })
            .ShouldNotHaveValidationErrorFor(x => x.PaidByMemberId);
    }
}
