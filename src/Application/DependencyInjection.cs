using Application.Abstractions;
using Application.Features.Sistema;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IQueryHandler<PingQuery, PingResponse>, PingHandler>();
        return services;
    }
}
