using Hexagrams.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Logging;
using Spenses.Api;
using Spenses.Api.Infrastructure;
using Spenses.Application;
using Spenses.Resources.Relational;
using Spenses.Shared.Common;

var builder = WebApplication.CreateBuilder(args);

builder.BuildConfiguration();

const string corsPolicyName = "AllowSpecificOrigins";

builder
    .AddWebApiServices(corsPolicyName)
    .AddIdentityServices()
    .AddAuthorizationServices();

builder.Services
    .AddApplicationServices()
    .AddRelationalServices(builder.Configuration.Require(ConfigConstants.SqlServerConnectionString));

builder.Services.AddAuthenticatedOpenApiDocument();

var app = builder.Build();

app.UseHttpLogging();
app.UseExceptionHandler();

if (builder.Environment.IsDevelopmentOrIntegrationTestEnvironment())
{
    IdentityModelEventSource.ShowPII = true;
}
else
{
    app.UseHsts();
}

app.AddSwaggerUi();

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization();

app.MapCustomHealthChecks("/health");

if (await app.Services.GetRequiredService<IFeatureManager>().IsEnabledAsync(FeatureNames.ErrorGeneration))
{
    app.UseMiddleware<ErrorGenerationMiddleware>();
}

app.Run();

namespace Spenses.Api
{
    // ReSharper disable once PartialTypeWithSinglePart
    // This allows this class to be used in as an entry point for integration tests.
    // See: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory
    public partial class Program;
}
