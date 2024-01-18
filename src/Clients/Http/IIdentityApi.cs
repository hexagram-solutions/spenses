using Refit;
using Spenses.Shared.Models.Identity;
using Spenses.Shared.Models.Me;

namespace Spenses.Client.Http;

public interface IIdentityApi
{
    [Post("/identity/login")]
    public Task<IApiResponse<LoginResult>> Login(LoginRequest request);

    [Post("/identity/login-with-2fa")]
    public Task<IApiResponse<LoginResult>> TwoFactorLogin(TwoFactorLoginRequest request);

    [Post("/identity/register")]
    public Task<IApiResponse<CurrentUser>> Register(RegisterRequest request);

    [Post("/identity/verify-email")]
    public Task<IApiResponse> VerifyEmail(VerifyEmailRequest request);

    [Post("/identity/resend-verification-email")]
    public Task<IApiResponse> ResendVerificationEmail(ResendVerificationEmailRequest request);

    [Post("/identity/logout")]
    public Task<IApiResponse> Logout();

    [Post("/identity/forgot-password")]
    public Task<IApiResponse> ForgotPassword(ForgotPasswordRequest request);

    [Post("/identity/reset-password")]
    public Task<IApiResponse> ResetPassword(ResetPasswordRequest request);
}
