using Hexagrams.Extensions.Authentication.OAuth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using IAccessTokenProvider = Hexagrams.Extensions.Authentication.OAuth.IAccessTokenProvider;

namespace Spenses.Client.Web.Infrastructure;

public class WebAssemblyAuthenticationAccessTokenProvider : IAccessTokenProvider
{
    private readonly IAccessTokenProviderAccessor _accessor;

    public WebAssemblyAuthenticationAccessTokenProvider(IAccessTokenProviderAccessor accessor)
    {
        _accessor = accessor;
    }

    public async Task<AccessTokenResponse> GetAccessTokenAsync(params string[] scopes)
    {
        var tokenResult =
            await _accessor.TokenProvider.RequestAccessToken(new AccessTokenRequestOptions { Scopes = scopes });

        tokenResult.TryGetToken(out var accessToken);

        var utcNow = DateTimeOffset.UtcNow;

        return new AccessTokenResponse
        {
            AccessToken = accessToken.Value,
            ExpiresIn = (int) accessToken.Expires.Subtract(utcNow).TotalSeconds,
            Scope = string.Join(' ', accessToken.GrantedScopes)
        };
    }
}
