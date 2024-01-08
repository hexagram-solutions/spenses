using System.Security.Claims;
using Hexagrams.Extensions.Common.Serialization;
using Microsoft.AspNetCore.Components.Authorization;
using Spenses.Application.Models.Authentication;
using Spenses.Client.Http;
using Spenses.Utilities.Security;

namespace Spenses.Web.Client;

public interface IAuthService
{
    public Task<LoginResult> Login(LoginRequest request);

    public Task<LoginResult> TwoFactorLogin(TwoFactorLoginRequest request);

    public Task Logout();
}

public class CookieAuthenticationStateProvider(IAuthApi authApi, IMeApi meApi)
    : AuthenticationStateProvider, IAuthService
{
    private bool _authenticated;

    public async Task<LoginResult> Login(LoginRequest request)
    {
        var result = await authApi.Login(request);

        if (result.Error is not null)
            return result.Error.Content!.FromJson<LoginResult>()!;

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return result.Content!;
    }

    public async Task<LoginResult> TwoFactorLogin(TwoFactorLoginRequest request)
    {
        var result = await authApi.TwoFactorLogin(request);

        if (result.Error is not null)
            return result.Error.Content!.FromJson<LoginResult>()!;

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        return result.Content!;
    }

    public async Task Logout()
    {
        await authApi.Logout();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;

        var user = new ClaimsPrincipal(new ClaimsIdentity());

        var userResponse = await meApi.GetMe();

        if (userResponse.Error is not null)
            return new AuthenticationState(user);

        var currentUser = userResponse.Content!;

        var claims = new List<Claim>
        {
            new(ApplicationClaimTypes.Name, currentUser.UserName),
            new(ApplicationClaimTypes.Email, currentUser.Email),
            new(ApplicationClaimTypes.EmailVerified, currentUser.EmailVerified.ToString())
        };

        user = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider)));

        _authenticated = true;

        return new AuthenticationState(user);
    }
}
