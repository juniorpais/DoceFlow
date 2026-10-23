using Domain.Common;

namespace Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IDomainEvent;
}
