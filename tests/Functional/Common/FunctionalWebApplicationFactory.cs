using Application.Abstractions;
using Infrastructure.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Functional.Common;

public sealed class FunctionalWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            foreach (var descriptor in services.Where(d => d.ServiceType == typeof(IHostedService)).ToList())
            {
                services.Remove(descriptor);
            }

            services.RemoveAll<IEventPublisher>();
            services.AddScoped<IEventPublisher, LogEventPublisher>();
        });
    }
}
