using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using Morris.Blazor.Validation;
using Spenses.App;
using Spenses.App.Components;
using Spenses.Shared.Validators.Identity;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.AddIdentityServices()
    .AddApiClients();

builder.Services
    .AddFluentUIComponents()
    .AddFormValidation(config => config.AddFluentValidation(typeof(LoginRequestValidator).Assembly));

await builder.Build().RunAsync();
