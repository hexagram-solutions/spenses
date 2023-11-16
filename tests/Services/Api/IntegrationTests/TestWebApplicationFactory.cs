using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Common;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api.IntegrationTests;

public class TestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentNames.IntegrationTest);

        builder.ConfigureTestServices(services =>
        {
            services.Configure<TestAuthenticationHandlerOptions>(options =>
            {
                options.DefaultUserIdentifier = "integration-test-user";
                options.DefaultUserEmail = "george@vandelayindustries.com";
                options.DefaultUserNickName = "George Costanza";
                options.DefaultUserIssuer = "self";
            });

            services.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
                .AddScheme<TestAuthenticationHandlerOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationScheme, _ => { });

            services.AddScoped<ICurrentUserService>(sp =>
            {
                var principal = sp.GetRequiredService<TestAuthenticationHandler>().GetClaimsPrincipal();

                return new MockCurrentUserService(principal);
            });
        });
    }
}

public class MockCurrentUserService(ClaimsPrincipal currentUser) : ICurrentUserService
{
    public ClaimsPrincipal CurrentUser { get; } = currentUser;
}
