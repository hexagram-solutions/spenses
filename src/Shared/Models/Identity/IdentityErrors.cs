namespace Spenses.Shared.Models.Identity;

public static class IdentityErrors
{
    public const string InvalidUserName = nameof(InvalidUserName);

    public const string InvalidEmail = nameof(InvalidEmail);

    public const string DuplicateUserName = nameof(DuplicateUserName);

    public const string DuplicateEmail = nameof(DuplicateEmail);

    public const string PasswordTooShort = nameof(PasswordTooShort);

    public const string PasswordTooCommon = nameof(PasswordTooCommon);

    public const string UserNameAsPassword = nameof(UserNameAsPassword);

    public const string EmailAsPassword = nameof(EmailAsPassword);

    public const string RecoveryCodeRedemptionFailed = nameof(RecoveryCodeRedemptionFailed);
}
