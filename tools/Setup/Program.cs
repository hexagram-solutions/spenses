using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spenses.Tools.Setup;
using Spenses.Tools.Setup.SeedData;

var config = new ConfigurationManager()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

await using var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder
        .AddConsole()
        .AddConfiguration(config))
    .AddTransient<IConfiguration>(_ => config)
    .AddTransient<DbSetupCommand>()
    .AddTransient<DbContextOptionsFactory>()
    .Scan(scan => scan
        .FromAssemblyOf<ISeedDataTask>()
        .AddClasses(classes => classes.AssignableTo<ISeedDataTask>())
        .AsImplementedInterfaces()
        .WithTransientLifetime())
    .BuildServiceProvider();

var command = serviceProvider.GetRequiredService<DbSetupCommand>();

return await command.InvokeAsync(args);
