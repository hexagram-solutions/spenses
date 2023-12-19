using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spenses.Resources.Communication.Azure;
using Spenses.Resources.Communication.Smtp;

namespace Spenses.Resources.Communication;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureEmailServices(this IServiceCollection services,
        IConfigurationSection emailConfiguration, string connectionString)
    {
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddEmailClient(connectionString);
        });

        services.Configure<EmailClientOptions>(emailConfiguration);

        services.AddTransient<IEmailClient, AzureEmailClient>();

        return services;
    }

    public static IServiceCollection AddSmtpEmailServices(this IServiceCollection services,
        IConfigurationSection emailConfiguration, IConfigurationSection smtpConfiguration)
    {
        services.Configure<EmailClientOptions>(emailConfiguration);
        services.Configure<SmtpOptions>(smtpConfiguration);

        return services.AddTransient<IEmailClient, SmtpEmailClient>();
    }
}
