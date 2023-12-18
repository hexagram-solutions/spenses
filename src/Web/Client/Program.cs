using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Spenses.Application.Common;
using Spenses.Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

var baseUrl = builder.Configuration.Require(ConfigConstants.SpensesApiBaseUrl);

if (builder.HostEnvironment.IsEnvironment(EnvironmentNames.Local))
    builder.Services.AddApiClients(baseUrl, false, TimeSpan.FromMilliseconds(500));
else
    builder.Services.AddApiClients(baseUrl, true);

var isLocalOrTestEnvironment =
    builder.HostEnvironment.IsEnvironment(EnvironmentNames.Local) ||
    builder.HostEnvironment.IsEnvironment(EnvironmentNames.Test);

builder.Services.AddStateManagement(isLocalOrTestEnvironment);

await builder.Build().RunAsync();
