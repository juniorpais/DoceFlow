using Application.Abstractions;
using Infrastructure.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Integration.Common;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing"); // carrega appsettings.Testing.json

        builder.ConfigureServices(services =>
        {
            // 1. Remover hosted services (bootstrap de mensageria, consumers)
            foreach (var descriptor in services.Where(d => d.ServiceType == typeof(IHostedService)).ToList())
            {
                services.Remove(descriptor);
            }

            // 2. Trocar publisher real por log
            services.RemoveAll<IEventPublisher>();
            services.AddScoped<IEventPublisher, LogEventPublisher>();

            // 3. IDbConnectionFactory NÃO é substituído nesta etapa (sem banco).
        });
    }
}
