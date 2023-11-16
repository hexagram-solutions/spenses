using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Spenses.Application.Common;
using Spenses.Client.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddAuth0Authentication(
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAuthority),
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdClientId),
    builder.Configuration.Require(ConfigConstants.SpensesOpenIdAudience));

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

var baseUrl = builder.Configuration.Require(ConfigConstants.SpensesApiBaseUrl);
var scopes = new[] { "openid", "profile", "email", "offline_access" };

if (builder.HostEnvironment.IsEnvironment(EnvironmentNames.Local))
    builder.Services.AddApiClients(baseUrl, scopes, false, TimeSpan.FromMilliseconds(500));
else
    builder.Services.AddApiClients(baseUrl, scopes, true);

var isLocalOrTestEnvironment =
    builder.HostEnvironment.IsEnvironment(EnvironmentNames.Local) ||
    builder.HostEnvironment.IsEnvironment(EnvironmentNames.Test);

builder.Services.AddStateManagement(isLocalOrTestEnvironment);

builder.Services.AddBlazoriseComponents();

await builder.Build().RunAsync();
