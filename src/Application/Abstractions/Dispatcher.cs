using System.Reflection;
using System.Runtime.ExceptionServices;
using FluentResults;

namespace Application.Abstractions;

/// <summary>Dispatcher CQRS sem mediator: resolve handlers do <see cref="IServiceProvider"/> via reflexão.</summary>
public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public Task<Result<TResult>> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return InvokeCommandHandler(command, cancellationToken);
    }

    public async Task<Result> Send(ICommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var result = await InvokeCommandHandler<CommandResult>(command, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess ? Result.Ok() : Result.Fail(result.Errors);
    }

    public Task<Result<TResult>> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);
        return InvokeQueryHandler(query, cancellationToken);
    }

    private Task<Result<TResult>> InvokeCommandHandler<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        var method = typeof(Dispatcher).GetMethod(nameof(InvokeCommandHandlerInternal), BindingFlags.Instance | BindingFlags.NonPublic)!;
        var generic = method.MakeGenericMethod(command.GetType(), typeof(TResult));
        return (Task<Result<TResult>>)Invoke(generic, [command, cancellationToken]);
    }

    private Task<Result<TResult>> InvokeCommandHandlerInternal<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResult>
    {
        var handler = serviceProvider.GetService(typeof(ICommandHandler<TCommand, TResult>)) as ICommandHandler<TCommand, TResult>
            ?? throw new InvalidOperationException($"Nenhum command handler registrado para {typeof(TCommand).Name}.");

        return handler.HandleAsync(command, cancellationToken);
    }

    private Task<Result<TResult>> InvokeQueryHandler<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        var method = typeof(Dispatcher).GetMethod(nameof(InvokeQueryHandlerInternal), BindingFlags.Instance | BindingFlags.NonPublic)!;
        var generic = method.MakeGenericMethod(query.GetType(), typeof(TResult));
        return (Task<Result<TResult>>)Invoke(generic, [query, cancellationToken]);
    }

    private Task<Result<TResult>> InvokeQueryHandlerInternal<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>
    {
        var handler = serviceProvider.GetService(typeof(IQueryHandler<TQuery, TResult>)) as IQueryHandler<TQuery, TResult>
            ?? throw new InvalidOperationException($"Nenhum query handler registrado para {typeof(TQuery).Name}.");

        return handler.HandleAsync(query, cancellationToken);
    }

    /// <summary>Invoca o método genérico desembrulhando <see cref="TargetInvocationException"/>, para que
    /// exceções lançadas na resolução do handler cheguem ao chamador com o tipo original.</summary>
    private object Invoke(MethodInfo method, object[] args)
    {
        try
        {
            return method.Invoke(this, args)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }
}
