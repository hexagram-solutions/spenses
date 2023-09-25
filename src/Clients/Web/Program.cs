using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
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

builder.Services.AddApiClients(builder.Configuration.Require(ConfigConstants.SpensesApiBaseUrl),
    new[] { "openid", "profile", "email" });

builder.Services.AddFluentUIComponents(options =>
{
    options.HostingModel = BlazorHostingModel.WebAssembly;
});

await builder.Build().RunAsync();
