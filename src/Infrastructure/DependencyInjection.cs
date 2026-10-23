using Application.Abstractions;
using Infrastructure.Data;
using Infrastructure.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddSingleton<IDbConnectionFactory>(new SqlConnectionFactory(connectionString));
        }

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        var options = new MessagingOptions();
        configuration.GetSection(MessagingOptions.SectionName).Bind(options);
        services.AddSingleton(options);

        var provider = options.Provider?.Trim();

        if (string.IsNullOrEmpty(provider) || provider.Equals("Log", StringComparison.OrdinalIgnoreCase))
        {
            services.AddScoped<IEventPublisher, LogEventPublisher>();
            services.AddScoped<IMessageBus, LogMessageBus>();
        }
        else if (provider.Equals("RabbitMq", StringComparison.OrdinalIgnoreCase)
              || provider.Equals("ServiceBus", StringComparison.OrdinalIgnoreCase))
        {
            // Falha só na resolução, para a API ainda subir; implementação na etapa de notificações.
            services.AddScoped<IEventPublisher>(_ => throw NaoSuportado(provider));
            services.AddScoped<IMessageBus>(_ => throw NaoSuportado(provider));
        }
        else
        {
            throw new InvalidOperationException($"Messaging:Provider desconhecido: '{provider}'.");
        }

        return services;
    }

    private static NotSupportedException NaoSuportado(string provider) =>
        new($"Provider {provider} será implementado na etapa de notificações");
}
