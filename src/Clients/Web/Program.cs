using Hexagrams.Extensions.Configuration;
using Spenses.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
using Spenses.Application.Common;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = builder.Configuration.Require(ConfigConstants.SpensesOpenIdAuthority);
    options.ProviderOptions.ClientId = builder.Configuration.Require(ConfigConstants.SpensesOpenIdClientId);
    options.ProviderOptions.ResponseType = "code";
});

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
