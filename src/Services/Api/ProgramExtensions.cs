using System.Security.Claims;
using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.AspNetCore;
using Spenses.Api.Infrastructure;
using Spenses.Application.Common;
using Spenses.Resources.Relational;
using Spenses.Utilities.Security.Services;
using Spenses.Resources.Relational.Infrastructure;

namespace Spenses.Api;

public static class ProgramExtensions
{
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

    private const string OpenApiDocumentTitle = "Spenses API";

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

    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddTransient<ICurrentUserService, HttpContextCurrentUserService>();

        return services;
    }

    public static IServiceCollection AddDbContextServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlServer(configuration.Require(ConfigConstants.SqlServerConnectionString)));

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

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
}
