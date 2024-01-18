using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Azure.Identity;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.FeatureManagement;
using PwnedPasswords.Validator;
using Spenses.Api.Infrastructure;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Application.Features.Identity;
using Spenses.Application.Services.Identity;
using Spenses.Application.Services.Identity.Passwords;
using Spenses.Resources.Communication;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Shared.Common;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api;

public static class ProgramExtensions
{
    private const string OpenApiDocumentTitle = "Spenses API";

    public static bool IsDevelopmentOrIntegrationTestEnvironment(this IWebHostEnvironment environment)
    {
        return environment.IsEnvironment(EnvironmentNames.Development) ||
            environment.IsEnvironment(EnvironmentNames.IntegrationTest);
    }

    public static WebApplicationBuilder BuildConfiguration(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopmentOrIntegrationTestEnvironment())
        {
            var appConfigurationConnectionString =
                builder.Configuration.Require(ConfigConstants.SpensesAppConfigurationConnectionString);

            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(appConfigurationConnectionString)
                    .ConfigureKeyVault(kv => { kv.SetCredential(new DefaultAzureCredential()); })
                    .ConfigureRefresh(refresh =>
                        refresh.Register(ConfigConstants.SpensesAppConfigurationSentinel, true));
            });
        }
        else
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        builder.Configuration.SetKeyDelimiters(":", "_", "-", ".");

        return builder;
    }

    public static WebApplicationBuilder AddWebApiServices(this WebApplicationBuilder builder,
        string corsPolicyName)
    {
        builder.Services
            .AddControllers(options =>
            {
                options.Filters.Add<ApplicationExceptionFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddRouting(opts =>
        {
            opts.LowercaseUrls = true;
            opts.LowercaseQueryStrings = true;
        });

        // Disables data annotations model validation, we only want to use FluentValidation
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        builder.Services.AddHttpLogging(_ => { });

        builder.Services.AddCors(opts =>
        {
            opts.AddPolicy(corsPolicyName,
                policy =>
                {
                    policy.WithOrigins(builder.Configuration.Collection(ConfigConstants.SpensesApiAllowedOrigins))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        builder.Services.AddFeatureManagement();

        return builder;
    }

    public static WebApplicationBuilder AddIdentityServices(this WebApplicationBuilder builder)
    {
        if (!builder.Environment.IsDevelopmentOrIntegrationTestEnvironment())
        {
            var blobStorageUri =
                new Uri(builder.Configuration.Require(ConfigConstants.SpensesDataProtectionBlobStorageSasUri));
            var keyIdentifier =
                new Uri(builder.Configuration.Require(ConfigConstants.SpensesDataProtectionKeyIdentifier));

            builder.Services.AddDataProtection()
                .SetApplicationName(builder.Configuration.Require(ConfigConstants.SpensesDataProtectionApplicationName))
                .PersistKeysToAzureBlobStorage(blobStorageUri)
                .ProtectKeysWithAzureKeyVault(keyIdentifier, new DefaultAzureCredential());
        }

        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies()
            .ApplicationCookie!.Configure(opt => opt.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
            });

        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddPwnedPasswordHttpClient(minimumFrequencyToConsiderPwned: 3)
            .AddStandardResilienceHandler();

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddPasswordValidator<UserNameAsPasswordValidator>()
            .AddPasswordValidator<EmailAsPasswordValidator>()
            .AddPasswordValidator<PwnedPasswordValidator<ApplicationUser>>()
            .AddPwnedPasswordErrorDescriber<PwnedPasswordErrorDescriber>();

        // We specify a minimum password length and no other requirements. We compare submitted passwords to a list
        // of common passwords to validate them instead.
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredUniqueChars = 3;
            options.Password.RequiredLength = 8;
        });

        builder.Services.AddTransient<IEmailSender<ApplicationUser>, IdentityEmailSender>();

        if (builder.Environment.IsDevelopmentOrIntegrationTestEnvironment())
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

        builder.Services.Configure<IdentityEmailOptions>(
            builder.Configuration.GetRequiredSection(ConfigConstants.SpensesIdentityEmailConfigurationSection));

        return builder;
    }

    public static IServiceCollection AddAuthenticatedOpenApiDocument(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument(document =>
        {
            document.Title = OpenApiDocumentTitle;
            document.SchemaSettings.FlattenInheritanceHierarchy = true;
            document.SchemaSettings.AllowReferencesWithProperties = true;
        });

        return services;
    }

    public static WebApplicationBuilder AddAuthorizationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<ICurrentUserService, HttpContextCurrentUserService>();
        builder.Services.AddTransient<ICurrentUserAuthorizationService, CurrentUserAuthorizationService>();

        builder.Services.Scan(scan => scan
            .FromAssemblyOf<HomeMemberRequirement>()
            .AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return builder;
    }

    public static IApplicationBuilder AddSwaggerUi(this IApplicationBuilder app)
    {
        app.UseOpenApi();

        app.UseSwaggerUi(config =>
        {
            config.DocumentTitle = OpenApiDocumentTitle;
        });

        return app;
    }

    public static IEndpointRouteBuilder MapCustomHealthChecks(this IEndpointRouteBuilder app, string pattern)
    {
        app.MapHealthChecks(pattern, new HealthCheckOptions { ResponseWriter = WriteResponse });

        return app;
    }

    private static Task WriteResponse(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();

        using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("status", healthReport.Status.ToString());
            jsonWriter.WriteString("totalDuration", healthReport.TotalDuration.ToString());
            jsonWriter.WriteStartObject("results");

            foreach (var entry in healthReport.Entries)
            {
                jsonWriter.WriteStartObject(entry.Key);
                jsonWriter.WriteString("status", entry.Value.Status.ToString());
                jsonWriter.WriteString("description", entry.Value.Description);
                jsonWriter.WriteString("duration", entry.Value.Duration.ToString());

                jsonWriter.WriteStartArray("tags");

                foreach (var tag in entry.Value.Tags)
                    jsonWriter.WriteStringValue(tag);

                jsonWriter.WriteEndArray();

                jsonWriter.WriteStartObject("data");

                foreach (var item in entry.Value.Data)
                {
                    jsonWriter.WritePropertyName(item.Key);

                    JsonSerializer.Serialize(jsonWriter, item.Value,
                        item.Value.GetType());
                }

                jsonWriter.WriteEndObject();

                if (entry.Value.Exception is not null)
                {
                    jsonWriter.WriteStartObject("exception");

                    jsonWriter.WriteString("type", entry.Value.Exception.GetType().ToString());
                    jsonWriter.WriteString("message", entry.Value.Exception.Message);
                    jsonWriter.WriteString("stackTrace", entry.Value.Exception.StackTrace);

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        return context.Response.WriteAsync(
            Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}
