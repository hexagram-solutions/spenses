using System.Text.Json.Serialization;
using Hexagrams.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Spenses.Api;
using Spenses.Api.Infrastructure;
using Spenses.Application.Common;
using Spenses.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.BuildConfiguration();

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<UserSyncFilter>();
        options.Filters.Add<ApplicationExceptionFilter>();
        options.ModelValidatorProviders.Clear(); // Disable data annotations model validation
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

//todo: and a-what is a-dis?
//services.AddScoped(provider =>
//{
//    var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
//    var loggerFactory = provider.GetService<ILoggerFactory>();

//    return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
//});

builder.Services.AddRouting(opts =>
{
    opts.LowercaseUrls = true;
    opts.LowercaseQueryStrings = true;
});

const string corsPolicyName = "AllowSpecificOrigins";

builder.Services.AddCors(opts =>
{
    opts.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.WithOrigins(builder.Configuration.Collection(ConfigConstants.SpensesApiAllowedOrigins))
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services
    .AddApplicationServices()
    .AddAuthorizationServices()
    .AddDbContextServices(builder.Configuration);

builder.Services.AddAuthenticatedOpenApiDocument(
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAuthority),
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAudience));

builder.Services.AddAuth0Authentication(
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAuthority),
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAudience));

var app = builder.Build();

app.UseHttpLogging();

if (app.Environment.IsEnvironment(EnvironmentNames.Local) ||
    app.Environment.IsEnvironment(EnvironmentNames.IntegrationTest) ||
    app.Environment.IsEnvironment(EnvironmentNames.Development))
{
    app.UseDeveloperExceptionPage();
    app.UseHttpLogging();

    IdentityModelEventSource.ShowPII = true;
}
else
{
    app.UseHsts();
    app.UseExceptionHandler("/error");
}

app.AddSwaggerUi(app.Configuration.Require(ConfigConstants.SpensesOpenIdClientId));

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.MapHealthChecks("/healthz");

app.Run();

namespace Spenses.Api
{
    // ReSharper disable once PartialTypeWithSinglePart
    // This allows this class to be used in as an entry point for integration tests.
    // See: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#basic-tests-with-the-default-webapplicationfactory
    public partial class Program
    {
    }
}
