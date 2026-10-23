using Application.Abstractions;
using Domain.Common;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>Publisher de fallback: apenas registra o evento em log (dev, testes e CI).</summary>
public sealed class LogEventPublisher(ILogger<LogEventPublisher> logger) : IEventPublisher
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
    {
        logger.LogInformation("[DomainEvent] {EventType} {@Event}", typeof(TEvent).Name, @event);
        return Task.CompletedTask;
    }
}
