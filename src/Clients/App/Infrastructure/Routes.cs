namespace Spenses.App.Infrastructure;

public static class Routes
{
    public static class Identity
    {
        public static string Login(string? returnUrl = null) => string.IsNullOrEmpty(returnUrl)
            ? "/login"
            : $"/login?returnUrl={returnUrl}";

        public static string TwoFactorLogin(string? returnUrl = null) => string.IsNullOrEmpty(returnUrl)
            ? "/two-factor-login"
            : $"/two-factor-login?returnUrl={returnUrl}";
        public static string SignUp(string? returnUrl = null) => string.IsNullOrEmpty(returnUrl)
            ? "/sign-up"
            : $"/sign-up?returnUrl={returnUrl}";

        public static string ForgotPassword => "/forgot-password";

        public static string ReSendConfirmationEmail => "resend-confirmation-email";
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
