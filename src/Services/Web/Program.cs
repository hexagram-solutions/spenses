using Auth0.AspNetCore.Authentication;
using Spenses.Application.Common;
using Spenses.Common.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration.Require(ConfigConstants.SpensesOpenIdDomain);
    options.ClientId = builder.Configuration.Require(ConfigConstants.SpensesOpenIdClientId);
    options.Scope = "openid profile email";
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Privacy"); //TODO: Require auth for the entire app
    options.Conventions.AuthorizePage("/Account/Logout");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
