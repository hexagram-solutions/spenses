using Refit;
using Spenses.Shared.Models.Authentication;

namespace Spenses.Client.Http;

public interface IAuthApi
{
    [Post("/auth/login")]
    public Task<IApiResponse<LoginResult>> Login(LoginRequest request);

    [Post("/auth/login-with-2fa")]
    public Task<IApiResponse<LoginResult>> TwoFactorLogin(TwoFactorLoginRequest request);

    [Post("/auth/logout")]
    public Task<IApiResponse> Logout();

    [Post("/auth/register")]
    public Task<IApiResponse> Register(RegisterRequest request);
}
