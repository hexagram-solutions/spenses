using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Spenses.Api.IntegrationTests;

public class TestAuthenticationHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserId { get; set; } = null!;
}

public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationHandlerOptions>
{
    public const string UserId = "UserId";
    public const string AuthenticationScheme = "Test";

    private readonly string _defaultUserId;

    public TestAuthenticationHandler( IOptionsMonitor<TestAuthenticationHandlerOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _defaultUserId = options.CurrentValue.DefaultUserId;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            // Extract User ID from the request headers if it exists, otherwise use the default User ID from the options.
            Context.Request.Headers.TryGetValue(UserId, out var userId)
                ? new Claim(ClaimTypes.NameIdentifier, userId.First()!)
                : new Claim(ClaimTypes.NameIdentifier, _defaultUserId)
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
