using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Spenses.Api;
using Spenses.Application.Common;
using Spenses.Application.Homes;
using Spenses.Common.Configuration;
using Spenses.Common.Extensions;
using Spenses.Resources.Relational;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetKeyDelimiters(":", "_", "-", ".");

builder.Services.AddControllers();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

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

builder.Services.AddHealthChecks();

var userCodeAssemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => a.GetName().Name!.StartsWith("Spenses"))
    .ToArray(); // todo: why doesn't Spenses.Application show up in assys?

builder.Services.AddAutoMapper((sp, cfg) =>
{
    cfg.ConstructServicesUsing(sp.GetRequiredService);
//}, userCodeAssemblies);
}, typeof(HomeQuery).Yield());

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(userCodeAssemblies));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<HomeQuery>());

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.Require(ConfigConstants.SqlServerConnectionString)));

builder.Services.AddAuthenticatedOpenApiDocument(
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAuthority),
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAudience));

builder.Services.AddAuth0Authentication(
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAuthority),
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAudience));

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpLogging();

if (app.Environment.IsEnvironment(EnvironmentNames.Local) ||
    app.Environment.IsEnvironment(EnvironmentNames.Test) ||
    app.Environment.IsEnvironment(EnvironmentNames.Development))
{
    app.UseDeveloperExceptionPage();
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

app.MapHealthChecks("/");

app.Run();
