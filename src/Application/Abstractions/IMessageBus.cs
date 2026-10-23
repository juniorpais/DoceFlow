namespace Application.Abstractions;

public interface IMessageBus
{
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand;

    Task PublishAsync<T>(T message, string? sessionId, CancellationToken cancellationToken)
        where T : class;
}
