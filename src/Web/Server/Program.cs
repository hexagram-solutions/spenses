using Hexagrams.Extensions.Configuration;
using Spenses.Application.Common;
using Spenses.Web.Client;
using Spenses.Web.Client.Components.Pages;
using Spenses.Web.Server;
using Spenses.Web.Server.Components;
using Spenses.Web.Server.Components.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services
    .AddDatabaseServices(builder.Configuration.Require(ConfigConstants.SqlServerConnectionString))
    .AddIdentityServices(ConfigConstants.SpensesDataProtectionApplicationName);

var baseUrl = builder.Configuration.Require(ConfigConstants.SpensesApiBaseUrl);

if (builder.Environment.IsEnvironment(EnvironmentNames.Local))
    builder.Services.AddApiClients(baseUrl, false, TimeSpan.FromMilliseconds(500));
else
    builder.Services.AddApiClients(baseUrl, true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsEnvironment(EnvironmentNames.Local))
{
    app.UseWebAssemblyDebugging();
    // TODO: Investigate. Necessary? Useful?
    app.UseMigrationsEndPoint();

    // https://github.com/dotnet/aspnetcore/issues/28174
    builder.WebHost.UseStaticWebAssets();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
