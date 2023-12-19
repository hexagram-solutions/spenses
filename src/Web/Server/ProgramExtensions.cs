using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common;
using Spenses.Resources.Communication;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Web.Server.Components.Account;

namespace Spenses.Web.Server;

public static class ProgramExtensions
{
    public static bool IsLocalOrIntegrationTestEnvironment(this IWebHostEnvironment environment)
    {
        return environment.IsEnvironment(EnvironmentNames.Local) ||
            environment.IsEnvironment(EnvironmentNames.IntegrationTest);
    }

    public static WebApplicationBuilder AddDatabaseServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.Require(ConfigConstants.SqlServerConnectionString)));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        return builder;
    }

    public static WebApplicationBuilder AddIdentityServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

        builder.Services.AddDataProtection()
            .SetApplicationName(builder.Configuration.Require(ConfigConstants.SpensesDataProtectionApplicationName));

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        if (builder.Environment.IsLocalOrIntegrationTestEnvironment())
        {
            builder.Services.AddSmtpEmailServices(
                builder.Configuration.GetRequiredSection(ConfigConstants.CommunicationEmailConfigurationSection),
                builder.Configuration.GetRequiredSection(ConfigConstants.CommunicationSmtpOptionsSection));
        }
        else
        {
            builder.Services.AddAzureEmailServices(
                builder.Configuration.GetRequiredSection(ConfigConstants.CommunicationEmailConfigurationSection),
                builder.Configuration.Require(ConfigConstants.AzureCommunicationServicesConnectionString));
        }

        
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityEmailSender>();

        return builder;
    }
}
