using Hexagrams.Extensions.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Spenses.App;
using Spenses.App.Components;
using Spenses.App.Identity;
using Spenses.Application.Common;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.AddIdentityServices()
    .AddApiClients();

var baseUrl = builder.Configuration.Require(ConfigConstants.SpensesApiBaseUrl);

// todo: replace with use of IAuthApi
builder.Services.AddHttpClient(
        "Auth",
        opt => opt.BaseAddress = new Uri(baseUrl))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
