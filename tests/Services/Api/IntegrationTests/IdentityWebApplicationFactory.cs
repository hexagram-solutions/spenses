using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Resources.Communication;
using Spenses.Shared.Common;

namespace Spenses.Api.IntegrationTests;

public class IdentityWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentNames.IntegrationTest);

        builder.ConfigureAppConfiguration(configure =>
        {
            configure.AddUserSecrets(GetType().Assembly);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<CapturingEmailClient>();
            services.AddTransient<IEmailClient>(sp => sp.GetRequiredService<CapturingEmailClient>());
        });
    }
}
