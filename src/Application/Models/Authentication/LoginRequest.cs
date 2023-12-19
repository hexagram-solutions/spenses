namespace Spenses.Application.Models.Authentication;

public record LoginRequest(
    string Email,
    string Password);

public record TwoFactorLoginRequest
{
    public string? TwoFactorCode { get; init; }

    public string? TwoFactorRecoveryCode { get; init; }

    public required bool TwoFactorRememberClient { get; init; }
}

public record LoginResult(
    bool Succeeded,
    bool RequiresTwoFactor = false,
    bool IsNotAllowed = false,
    bool IsLockedOut = false);
