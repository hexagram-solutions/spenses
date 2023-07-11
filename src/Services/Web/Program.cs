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

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages()
    .RequireAuthorization();

app.Run();
