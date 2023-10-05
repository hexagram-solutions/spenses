using Hexagrams.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Spenses.Api;
using Spenses.Application;
using Spenses.Application.Common;
using Spenses.Resources.Relational;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.BuildConfiguration();

const string corsPolicyName = "AllowSpecificOrigins";

builder.Services
    .AddWebApiServices(builder.Configuration, corsPolicyName)
    .AddApplicationServices()
    .AddAuthorizationServices()
    .AddRelationalServices(builder.Configuration.Require(ConfigConstants.SqlServerConnectionString));

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

app.MapCustomHealthChecks("/healthz");

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
