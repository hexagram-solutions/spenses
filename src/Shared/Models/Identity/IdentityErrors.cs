namespace Spenses.Shared.Models.Identity;

public static class IdentityErrors
{
    public static class Login
    {
        public const string InvalidCredentials = nameof(InvalidCredentials);

        public const string EmailVerificationRequired = nameof(EmailVerificationRequired);

        public const string LockedOut = nameof(LockedOut);

        public const string RecoveryCodeRedemptionFailed = nameof(RecoveryCodeRedemptionFailed);
    }

    public static class Register
    {
        public const string InvalidUserName = nameof(InvalidUserName);

        public const string InvalidEmail = nameof(InvalidEmail);

        public const string DuplicateUserName = nameof(DuplicateUserName);

        public const string DuplicateEmail = nameof(DuplicateEmail);
    }

    public static class Password
    {
        public const string PasswordTooShort = nameof(PasswordTooShort);

        public const string PwnedPassword = nameof(PwnedPassword);

        public const string UserNameAsPassword = nameof(UserNameAsPassword);

        public const string EmailAsPassword = nameof(EmailAsPassword);

        public const string PasswordMismatch = nameof(PasswordMismatch);
    }

    public static class EmailVerification
    {
        public const string InvalidToken = nameof(InvalidToken);
    }
}
