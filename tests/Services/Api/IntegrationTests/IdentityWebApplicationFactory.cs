using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Api.Infrastructure;
using Spenses.Api.IntegrationTests.Identity.Services;
using Spenses.Resources.Communication;
using Spenses.Resources.Relational;
using Spenses.Shared.Common;
using Spenses.Tools.Setup;
using Spenses.Utilities.Security.Services;
using Testcontainers.MsSql;

namespace Spenses.Api.IntegrationTests;

public class IdentityWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Hunter2!")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentNames.IntegrationTest);

        builder.ConfigureAppConfiguration(configure => { configure.AddUserSecrets(GetType().Assembly); });

        builder.ConfigureTestServices(services =>
        {
            AddTestDbContext(services)
                .AddDataSeeder();

            services.AddSingleton<UserContextProvider>();

            services.AddSingleton<SystemUserContext>();
            services.AddTransient(sp => sp.GetRequiredService<UserContextProvider>().GetContext());

            services.AddSingleton<CapturingEmailClient>();
            services.AddTransient<IEmailClient>(sp => sp.GetRequiredService<CapturingEmailClient>());
        });
    }

    private IServiceCollection AddTestDbContext(IServiceCollection services)
    {
        var descriptorType =
            typeof(DbContextOptions<ApplicationDbContext>);

        var descriptor = services
            .SingleOrDefault(s => s.ServiceType == descriptorType);

        if (descriptor is not null)
            services.Remove(descriptor);

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(_dbContainer.GetConnectionString()));

        return services;
    }
}
