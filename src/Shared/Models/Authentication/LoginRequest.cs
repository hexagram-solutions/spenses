namespace Spenses.Shared.Models.Authentication;

public record LoginRequest(
    string Email,
    string Password);

public record TwoFactorLoginRequest(string? Code, string? RecoveryCode, bool RememberClient);

public record LoginResult(
    bool Succeeded,
    bool RequiresTwoFactor = false,
    bool IsNotAllowed = false,
    bool IsLockedOut = false);

public record CurrentUser
{
    public required string UserName { get; init; }

    public required string Email { get; init; }

    public required bool EmailVerified { get; init; }
}

public record RegisterRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
