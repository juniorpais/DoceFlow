using Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Events;

/// <summary>Message bus de fallback: apenas registra as mensagens em log.</summary>
public sealed class LogMessageBus(ILogger<LogMessageBus> logger) : IMessageBus
{
    public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        logger.LogInformation("[MessageBus.Send] {CommandType} {@Command}", typeof(TCommand).Name, command);
        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(T message, string? sessionId, CancellationToken cancellationToken)
        where T : class
    {
        logger.LogInformation(
            "[MessageBus.Publish] {MessageType} Sessao={SessionId} {@Message}",
            typeof(T).Name, sessionId, message);
        return Task.CompletedTask;
    }
}
