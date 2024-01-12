using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Spenses.App;
using Spenses.App.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder
    .AddWebServices()
    .AddIdentityServices()
    .AddApiClients()
    .AddStateManagement();

await builder.Build().RunAsync();
