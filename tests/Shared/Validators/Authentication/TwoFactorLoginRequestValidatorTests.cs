using FluentValidation.TestHelper;
using Spenses.Shared.Models.Authentication;
using Spenses.Shared.Validators.Authentication;

namespace Spenses.Shared.Tests.Validators.Authentication;

public class TwoFactorLoginRequestValidatorTests
{
    private readonly TwoFactorLoginRequestValidator _validator = new();

    [Fact]
    public void Code_is_required_if_recovery_code_is_null_or_empty()
    {
        var model = new TwoFactorLoginRequest(null, null, true);

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.Code);

        _validator.TestValidate(model with { Code = "123456" })
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Recovery_code_is_required_if_code_is_null_or_empty()
    {
        var model = new TwoFactorLoginRequest(null, null, true);

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(x => x.RecoveryCode);

        _validator.TestValidate(model with { RecoveryCode = "reco-very" })
            .ShouldNotHaveAnyValidationErrors();
    }
}
