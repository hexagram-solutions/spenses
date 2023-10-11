using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Azure.Identity;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.AspNetCore;
using Spenses.Api.Infrastructure;
using Spenses.Application.Common;
using Spenses.Application.Features.Homes.Authorization;
using Spenses.Utilities.Security.Services;

namespace Spenses.Api;

public static class ProgramExtensions
{
    private const string OpenApiDocumentTitle = "Spenses API";

    public static ConfigurationManager BuildConfiguration(this ConfigurationManager configuration)
    {
        var environment = configuration.Require(ConfigConstants.AspNetCoreEnvironment);

        if (environment != EnvironmentNames.Local &&
            environment != EnvironmentNames.IntegrationTest)
        {
            var appConfigurationConnectionString =
                configuration.Require(ConfigConstants.SpensesAppConfigurationConnectionString);

            configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(appConfigurationConnectionString)
                    .ConfigureKeyVault(kv => { kv.SetCredential(new DefaultAzureCredential()); })
                    .ConfigureRefresh(refresh =>
                        refresh.Register(ConfigConstants.SpensesAppConfigurationSentinel, true));
            });
        }

        configuration.SetKeyDelimiters(":", "_", "-", ".");

        return configuration;
    }

    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration,
        string corsPolicyName)
    {
        services
            .AddControllers(options =>
            {
                options.Filters.Add<UserSyncFilter>();
                options.Filters.Add<ApplicationExceptionFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddRouting(opts =>
        {
            opts.LowercaseUrls = true;
            opts.LowercaseQueryStrings = true;
        });

        // Disables data annotations model validation, we only want to use FluentValidation
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
        });

        services.AddCors(opts =>
        {
            opts.AddPolicy(corsPolicyName,
                policy =>
                {
                    policy.WithOrigins(configuration.Collection(ConfigConstants.SpensesApiAllowedOrigins))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        return services;
    }

    public static IServiceCollection AddAuth0Authentication(this IServiceCollection services, string authority,
        string audience)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                    ValidAudience = audience,
                    ValidIssuer = authority
                };
            });

        return services;
    }

    public static IServiceCollection AddAuthenticatedOpenApiDocument(this IServiceCollection services, string authority,
        string audience)
    {
        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument(document =>
        {
            document.Title = OpenApiDocumentTitle;
            document.FlattenInheritanceHierarchy = true;
            document.AllowReferencesWithProperties = true;

            var authorityUri = new Uri(authority);
            var authorizationUrl = new Uri(authorityUri, $"authorize?audience={audience}").ToString();
            var tokenUrl = new Uri(authorityUri, "oauth/token").ToString();

            document.AddSecurity(JwtBearerDefaults.AuthenticationScheme, Enumerable.Empty<string>(),
                new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Description = "Bearer Authentication",
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = authorizationUrl,
                            TokenUrl = tokenUrl,
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "Allows the user be uniquely identified" },
                                { "profile", "Basic profile information" },
                                { "email", "Email and verification information" }
                            }
                        }
                    }
                });
        });

        return services;
    }

    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddTransient<ICurrentUserService, HttpContextCurrentUserService>();
        services.AddTransient<ICurrentUserAuthorizationService, CurrentUserAuthorizationService>();

        services.Scan(scan => scan
            .FromAssemblyOf<HomeMemberRequirement>()
            .AddClasses(classes => classes.AssignableTo<IAuthorizationHandler>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IApplicationBuilder AddSwaggerUi(this IApplicationBuilder app, string clientId)
    {
        app.UseOpenApi();

        app.UseSwaggerUi3(config =>
        {
            config.DocumentTitle = OpenApiDocumentTitle;

            config.OAuth2Client = new OAuth2ClientSettings
            {
                AppName = OpenApiDocumentTitle,
                ClientId = clientId,
                ClientSecret = string.Empty,
                UsePkceWithAuthorizationCodeGrant = true
            };
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
