using Application;
using Application.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Unit.Application;

public sealed class DependencyInjectionTests
{
    private static readonly Type[] HandlerDefinitions =
        [typeof(ICommandHandler<,>), typeof(IQueryHandler<,>)];

    [Fact(DisplayName = "Given Application assembly When calling AddApplication Then every concrete handler is registered")]
    public void Given_ApplicationAssembly_When_AddingApplication_Then_EveryHandlerIsRegistered()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        var assembly = typeof(IDispatcher).Assembly;
        var handlers = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && HandlerDefinitions.Contains(i.GetGenericTypeDefinition()))
                .Select(i => (Implementation: t, Service: i)))
            .ToList();

        handlers.Should().NotBeEmpty("a feature Sistema/Ping precisa existir para validar o pipeline");
        foreach (var (implementation, service) in handlers)
        {
            services.Should().Contain(
                d => d.ServiceType == service && d.ImplementationType == implementation,
                "{0} deve estar registrado como {1}",
                implementation.Name,
                service.Name);
        }
    }

    [Fact(DisplayName = "Given a service collection When calling AddApplication Then IDispatcher is registered")]
    public void Given_ServiceCollection_When_AddingApplication_Then_DispatcherIsRegistered()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        services.Should().Contain(d => d.ServiceType == typeof(IDispatcher));
    }
}
