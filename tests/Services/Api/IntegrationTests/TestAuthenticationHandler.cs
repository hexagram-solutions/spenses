using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spenses.Utilities.Security;

namespace Spenses.Api.IntegrationTests;

public class TestAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserIdentifier { get; set; } = null!;

    public string DefaultUserEmail { get; set; } = null!;

    public string DefaultUserNickName { get; set; } = null!;

    public string DefaultUserIssuer { get; set; } = null!;
}

public class TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationHandlerOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<TestAuthenticationHandlerOptions>(options, logger, encoder)
{
    public const string AuthenticationScheme = "Test";

    // ReSharper disable once ReplaceWithPrimaryConstructorParameter
    private readonly IOptionsMonitor<TestAuthenticationHandlerOptions> _options = options;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var principal = GetClaimsPrincipal();

        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }

    public ClaimsPrincipal GetClaimsPrincipal()
    {
        var claims = new List<Claim> {
            new (ApplicationClaimTypes.Identifier, _options.CurrentValue.DefaultUserIdentifier),
            new(ApplicationClaimTypes.NickName, _options.CurrentValue.DefaultUserNickName),
            new(ApplicationClaimTypes.Issuer, _options.CurrentValue.DefaultUserIssuer),
            new(ApplicationClaimTypes.Email, _options.CurrentValue.DefaultUserEmail),
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        return principal;
    }
}
