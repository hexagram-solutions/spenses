using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Spenses.Application.Common;
using Spenses.Application.Homes;
using Spenses.Common.Configuration;
using Spenses.Common.Extensions;
using Spenses.Resources.Relational;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetKeyDelimiters(":", "_", "-", ".");

builder.Services.AddControllers();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddRouting(opts =>
{
    opts.LowercaseUrls = true;
    opts.LowercaseQueryStrings = true;
});

builder.Services.AddHealthChecks();

var userCodeAssemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => a.GetName().Name!.StartsWith("Spenses"))
    .ToArray(); // todo: why doesn't Spenses.Application show up in assys?

builder.Services.AddAutoMapper((sp, cfg) =>
{
    cfg.ConstructServicesUsing(sp.GetRequiredService);
//}, userCodeAssemblies);
}, typeof(HomeQuery).Yield());

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(userCodeAssemblies));;
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<HomeQuery>());

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.Require(ConfigConstants.SqlServerConnectionString)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/");

app.Run();
