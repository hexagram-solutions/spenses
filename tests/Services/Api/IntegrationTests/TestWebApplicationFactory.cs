using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Application.Common;

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
                options.DefaultUserName = "George Costanza";
                options.DefaultUserIssuer = "self";
            });

            services.AddAuthentication(TestAuthenticationHandler.AuthenticationScheme)
                .AddScheme<TestAuthenticationHandlerOptions, TestAuthenticationHandler>(
                    TestAuthenticationHandler.AuthenticationScheme, _ => { });
        });
    }
}
