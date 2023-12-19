namespace Spenses.Application.Models.Authentication;

public record LoginRequest(
    string Email,
    string Password,
    string? TwoFactorCode = null,
    string? TwoFactorRecoveryCode = null);

public record LoginResult(
    bool Succeeded,
    bool? RequiresTwoFactor = null,
    bool? IsNotAllowed = null,
    bool? IsLockedOut = null);
