using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spenses.Resources.Relational;
using Spenses.Resources.Relational.Models;
using Spenses.Tools.Setup;
using Spenses.Tools.Setup.SeedData;
using Spenses.Utilities.Security.Services;

var config = new ConfigurationManager()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var services = new ServiceCollection()
    .AddLogging(builder => builder
        .AddConsole()
        .AddConfiguration(config))
    .AddTransient<IConfiguration>(_ => config)
    .AddTransient<DbSetupCommand>()
    .AddTransient<DbContextOptionsFactory>()
    .AddTransient<ICurrentUserService, SystemCurrentUserService>()
    .Scan(scan => scan
        .FromAssemblyOf<ISeedDataTask>()
        .AddClasses(classes => classes.AssignableTo<ISeedDataTask>())
        .AsImplementedInterfaces()
        .WithTransientLifetime());

services
    .AddDbContext<ApplicationDbContext>()
    .AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

await using var serviceProvider = services.BuildServiceProvider();

var command = serviceProvider.GetRequiredService<DbSetupCommand>();

return await command.InvokeAsync(args);
