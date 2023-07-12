using Auth0.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common;
using Spenses.Application.Homes;
using Spenses.Common.Configuration;
using Spenses.Common.Extensions;
using Spenses.Resources.Relational;
using Spenses.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration.Require(ConfigConstants.SpensesOpenIdDomain);
    options.ClientId = builder.Configuration.Require(ConfigConstants.SpensesOpenIdClientId);
    options.Scope = "openid profile email";
});

builder.Services
    .AddRazorPages(options =>
    {
        // Sets default startup page
        options.Conventions.AddPageRoute("/homes/index", "");
    })
    .AddRazorRuntimeCompilation()
    // for htmx-form validation
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = false;
    });

builder.Services.AddRouting(opts =>
{
    opts.LowercaseUrls = true;
    opts.LowercaseQueryStrings = true;
});

var userCodeAssemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => a.GetName().Name!.StartsWith("Spenses"))
    .ToArray(); // todo: why doesn't Spenses.Application show up in assys?

builder.Services.AddAutoMapper((sp, cfg) =>
{
    cfg.ConstructServicesUsing(sp.GetRequiredService);
    //}, userCodeAssemblies);
}, typeof(HomeQuery).Yield());

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(userCodeAssemblies));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<HomeQuery>());

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.Require(ConfigConstants.SqlServerConnectionString)));

builder.Services.AddScoped<IViewRenderingService, ViewRenderingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseDefaultFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages()
    .RequireAuthorization();

app.Run();
