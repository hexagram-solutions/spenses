namespace Spenses.App.Infrastructure;

public static class Routes
{
    public const string Root = "/";

    public static class Identity
    {
        public static string Login(string? returnUrl = null) => string.IsNullOrEmpty(returnUrl)
            ? "/login"
            : $"/login?returnUrl={returnUrl}";

        public static string TwoFactorLogin(string? returnUrl = null) => string.IsNullOrEmpty(returnUrl)
            ? "/two-factor-login"
            : $"/two-factor-login?returnUrl={returnUrl}";

        public static string SignUp => "/sign-up";

        public static string ForgotPassword => "/forgot-password";

        public static string EmailVerificationRequired => "/email-verification-required";

        public static string VerifyEmail => "/verify-email";
    }

    public static class Homes
    {
        public static string Dashboard(Guid homeId) => $"/homes/{homeId}/dashboard";
        public static string Expenses(Guid homeId) => $"/homes/{homeId}/expenses";
        public static string Payments(Guid homeId) => $"/homes/{homeId}/payments";
        public static string Insights(Guid homeId) => $"/homes/{homeId}/insights";
        public static string Settings(Guid homeId) => $"/homes/{homeId}/settings";
    }
}
